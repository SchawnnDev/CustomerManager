using CustomerManagement.Data;
using CustomerManagement.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using CustomerManagement.Enums;
using CustomerManagement.IO;

namespace CustomerManager
{
    public class Program
    {

        public static List<Customer> Customers = new List<Customer>();

        static void Main(string[] args)
        {
            PluginManager.LoadPlugins();


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

            PluginManager.GetActivePlugin().Init();
            Customers = PluginManager.GetActivePlugin().GetCustomers();

            Console.WriteLine("App is ready to use! :)");

        }

        public static bool ExecuteCommands(string[] args)
        {

            if (args.Length == 0 || args.Length == 1)
            {
                Help();
                return false;
            }
            else if (args.Length > 1)
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
                        } else if (!PluginManager.ChoosePlugin(args[0]))
                        {
                            SendCorrectUsage("");
                            Console.WriteLine($"Existing Database Types: {string.Join(", ",PluginManager.GetPluginNames())}");
                            return false;
                        }


                        PluginManager.GetActivePlugin().SetDataSource(args[1]);
                        break;
                }

            }

            switch (args.Length)
            {
                case 3:
                    switch (args[2])
                    {
                        case "display":
                            Start();
                            DisplayData.Display(Customers);
                            return true;
                        case "reset":
                            PluginManager.GetActivePlugin().Reset();
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

                case 4:
                    switch (args[2])
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
                case 5:
                    switch (args[2])
                    {
                        case "import":
                            SendCorrectUsage("[import] [path] [customer | address]  [startLine]");
                            return false;
                        case "delete":

                            if (!int.TryParse(args[4], out int id))
                            {
                                SendCorrectUsage("[delete] [customer | address] [id]");
                                throw new ApplicationException("ID must be a valid number.");
                            }


                            switch (args[3])
                            {
                                case "customer":
                                    PluginManager.GetActivePlugin().DeleteCustomer(id);
                                    break;

                                case "address":
                                    PluginManager.GetActivePlugin().DeleteShippingAddress(id);
                                    break;

                                default:
                                    SendCorrectUsage("[delete] [customer | address] [id]");
                                    throw new ApplicationException("Argument n4 must be 'customer' or 'address'.");

                            }

                            Start();
                            return true;


                    }

                    break;
                case 6 when !args[2].Equals("import"):
                    Help();
                    return false;
                case 6 when !File.Exists(args[3]):
                    throw new ApplicationException("File does not exist.");
                case 6:
                {
                    if (!int.TryParse(args[5], out int startLine))
                        throw new ApplicationException("StartLine must be a number.");

                    switch (args[4])
                    {
                        case "customer":
                            Start();
                            var customers = FileManager.ImportCustomers(args[3], startLine);
                            Console.WriteLine($"Imported {customers.Count} customers from {args[3]}");
                            DataManager.AddWithoutDoubles(Customers, customers);
                            Console.WriteLine($"Saved {PluginManager.GetActivePlugin().SaveCustomers(customers)} customers to db!");
                            return true;
                        case "address":
                            Start();
                            var shippingAddresses = FileManager.ImportShippingAddress(args[3], startLine);
                            Console.WriteLine($"Imported {shippingAddresses.Count} shipping addresses from {args[3]}");
                            var toBeSaved = Utils.SearchCustomersForShippingAddresses(Customers, shippingAddresses, SearchType.Name);

                            if (toBeSaved.Count != 0)
                            {
                                Console.Write($"Trying to save {toBeSaved.Count} shipping addresses to DB... ");
                                Console.WriteLine($"Success! {PluginManager.GetActivePlugin().SaveShippingAddresses(toBeSaved)} have been saved!");
                            }
                            else
                                Console.WriteLine("No customers were found for these addresses! Please import them before!");

                            return true;
                        default:
                            Console.WriteLine("Type does not exist!");
                            return false;
                    }

                }
            }

            return true;
        }

        private static void Help()
        {
            Console.WriteLine("Syntax: CustomerManager.exe [databaseType] [dataSource | ? | help] [test | import | reset | delete | display] [path | id] [options]");
            Console.WriteLine("    ? or help:     Show help");
            Console.WriteLine("    databaseType:       Database Type: " + string.Join(",",PluginManager.GetPluginNames()));
            Console.WriteLine("    dataSource:    SQL Server Name, Data Source name");
            Console.WriteLine("    start:         Only start app");
            Console.WriteLine("    import:        Import infos from .csv file : import [path] [customer | address] [startLine]");
            Console.WriteLine("    delete:        Delete user or shipping addresses by id : delete [customer | address] [id]");
            Console.WriteLine("    reset:         Reset SQL Database");
            Console.WriteLine("    display:       Display all data in DB.");
        }

        private static void SendCorrectUsage(string text)
        {
            Console.WriteLine($"Correct usage: CustomerManager.exe [databaseType] [dataSource] {text}");
        }



    }

}
