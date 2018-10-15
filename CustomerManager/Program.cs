using CustomerManagement.Data;
using CustomerManagement.Utils;
using CustomerManager.Data;
using System;
using System.Collections.Generic;
using System.IO;

namespace CustomerManager
{
    class Program
    {

        public static List<Customer> Customers = new List<Customer>();

        static void Main(string[] args)
        {
            DbManager.DatabaseName = "CustomerManager";
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            try
            {
                ExecuteCommands(args);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occurred: {e.Message}");
            }

        }

        private static void Start()
        {

            Console.WriteLine("Loading app CustomerManager...");

            DbManager.Init();
            Customers = DbManager.LoadData();

            Console.WriteLine("App is ready to use! :)");

        }

        public static bool ExecuteCommands(string[] args)
        {

            if (args.Length == 0)
            {
                Help();
                return false;
            }
            else if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "?":
                    case "help":
                        Help();
                        return false;
                    default:
                        if (args.Length == 1)
                        {
                            Help();
                            return false;
                        }

                        DbManager.DataSource = args[0];
                        break;
                }

            }

            if (args.Length == 2)
            {
                switch (args[1])
                {
                    case "display":
                        Start();
                        DisplayData.Display(Customers);
                        return true;
                    case "reset":
                        DbManager.Reset();
                        Start();
                        return true;
                    case "test":
                        Console.WriteLine("Starting app...");
                        Start();
                        return true;
                    case "import":
                        SendCorrectUsage("[import] [path] [customer | address] [startLine]");
                        return false;
                    case "delete":
                        SendCorrectUsage("[delete] [customer | address] [id]");
                        return false;
                    default:
                        Help();
                        return false;
                }
            }
            else if (args.Length == 3)
            {

                switch (args[1])
                {
                    case "delete":
                        SendCorrectUsage("[delete] [customer | address] [id]");
                        break;
                    case "import":
                        SendCorrectUsage("[import] [path] [customer | address]  [startLine]");
                        break;
                    default:
                        Help();
                        break;
                }

                return false;



            }
            else if (args.Length == 4)
            {

                switch (args[1])
                {
                    case "import":
                        SendCorrectUsage("[import] [path] [customer | address]  [startLine]");
                        return false;
                    case "delete":

                        if (!int.TryParse(args[3], out int id))
                        {
                            SendCorrectUsage("[delete] [customer | address] [id]");
                            throw new ApplicationException("ID must be a valid number.");
                        }


                        switch (args[2])
                        {
                            case "customer":
                                DbManager.DeleteCustomer(id);
                                break;

                            case "address":
                                DbManager.DeleteShippingAddress(id);
                                break;

                            default:
                                SendCorrectUsage("[delete] [customer | address] [id]");
                                throw new ApplicationException("Argument n3 must be 'customer' or 'address'.");

                        }

                        Start();
                        return true;


                }
            }
            else if (args.Length == 5)
            {

                if (!args[1].Equals("import"))
                {
                    Help();
                    return false;
                }

                if (!File.Exists(args[2]))
                    throw new ApplicationException("File does not exist.");
                if (!int.TryParse(args[4], out int startLine))
                    throw new ApplicationException("StartLine must be a number.");

                switch (args[3])
                {
                    case "customer":
                        Start();
                        List<Customer> customers = FileManager.ImportCustomers(args[2], startLine);
                        Console.WriteLine($"Imported {customers.Count} customers from {args[2]}");
                        DataManager.AddWithoutDoubles(Customers, customers);
                        Console.WriteLine($"Saved {DbManager.SaveCustomersToDB(customers)} customers to db!");
                        return true;
                    case "address":
                        Start();
                        List<ShippingAddress> shippingAddresses = FileManager.ImportShippingAddress(args[2], startLine);
                        Console.WriteLine($"Imported {shippingAddresses.Count} shipping addresses from {args[2]}");
                        List<ShippingAddress> toBeSaved = Utils.SearchCustomersForShippingAddresses(Customers, shippingAddresses, SearchType.Name);

                        if (toBeSaved.Count != 0)
                        {
                            Console.Write($"Trying to save {toBeSaved.Count} shipping addresses to DB... ");
                            Console.WriteLine($"Success! {DbManager.SaveShippingAddressesToDB(toBeSaved)} have been saved!");
                        }
                        else
                            Console.WriteLine("No customers were found for these addresses! Please import them before!");

                        return true;
                    default:
                        Console.WriteLine("Type does not exist!");
                        return false;
                }

            }

            return true;
        }

        private static void Help()
        {
            Console.WriteLine("Syntax: CustomerManager.exe [dataSource | ? | help] [test | import | reset | delete | display] [path | id] [options]");
            Console.WriteLine("    ? or help:     Show help");
            Console.WriteLine("    dataSoure:     SQL Server Name, Data Source name");
            Console.WriteLine("    start:         Only start app");
            Console.WriteLine("    import:        Import infos from .csv file : import [path] [customer | address] [startLine]");
            Console.WriteLine("    delete:        Delete user or shipping addresses by id : delete [customer | address] [id]");
            Console.WriteLine("    reset:         Reset SQL Database");
            Console.WriteLine("    display:       Display all data in DB.");
        }

        private static void SendCorrectUsage(string text)
        {
            Console.WriteLine($"Correct usage: CustomerManager.exe [dataSource] {text}");
        }



    }

}
