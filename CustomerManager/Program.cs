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

            DBManager.DatabaseName = "CustomerManager";

            ExecuteCommands(args);

        }

        private static void Start()
        {

            Console.WriteLine("Loading app CustomerManager...");

            DBManager.Init();
            DataManager.Init(DBManager.LoadData());

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
                        //TODO:
                        string[] a = args[0].Split(',');
                        DBManager.DataSource = a[0] + @"\" + a[1]; // Maybe check if its wrong?
                        break;
                }

            }

            if (args.Length == 2)
            {
                switch (args[1])
                {
                    case "reset":
                        DBManager.Reset();
                        Start();
                        return true;
                    case "start":
                        Console.WriteLine("Starting app...");
                        Start();
                        return true;
                    case "import":
                        Console.WriteLine("Correct using : CustomerManager.exe [dataSource] [import] [path] [customer | address] [startLine]");
                        return false;
                    case "delete":
                        Console.WriteLine("Correct using : CustomerManager.exe [dataSource] [delete] [id]");
                        return false;
                    default:
                        Help();
                        return false;
                }
            }
            else if (args.Length == 3)
            {
                if (args[1].Equals("delete"))
                {

                    if (!int.TryParse(args[2], out int id))
                    {
                        Console.WriteLine("Correct using : CustomerManager.exe [dataSource] [delete] [id]");
                        Console.WriteLine("ID MUST BE AN VALID NUMBER!!");
                        return false;
                    }

                    Start();
                    DBManager.DeleteCustomer(id, true);

                    return true;
                }
                else if (args[1].Equals("import"))
                {
                    Console.WriteLine("Correct using : CustomerManager.exe [dataSource] [import] [path] [customer | address]  [startLine]");
                    return false;
                }
                else
                {
                    Help();
                    return false;
                }
            }
            else if (args.Length == 4 && args[1].Equals("import"))
            {
                Console.WriteLine("Correct using : CustomerManager.exe [dataSource] [import] [path] [customer | address]  [startLine]");
                return false;
            }
            else if (args.Length == 5)
            {

                if (args[1].Equals("import"))
                {

                    if (File.Exists(args[2]))
                    {

                        if (int.TryParse(args[4], out int startLine))
                        {

                            string type = args[3];

                            if (type.Equals("customer"))
                            {

                                Start();
                                List<Customer> customers = FileManager.ImportCustomers(args[2], startLine);
                                Console.WriteLine("Imported " + customers.Count + " customers from " + args[2]);
                                DataManager.AddWithoutDoubles(customers);
                                int count = DBManager.SaveCustomersToDB(customers);
                                Console.WriteLine("Saved " + count + " customers to db!");
                                return true;
                            }
                            else if (type.Equals("address"))
                            {
                                Start();
                                List<ShippingAddress> shippingAddresses = FileManager.ImportShippingAddress(args[2], startLine);
                                Console.WriteLine("Imported " + shippingAddresses.Count + " shipping addresses from " + args[2]);
                                List<ShippingAddress> toBeSaved = Utils.Utils.SearchCustomersForShippingAddresses(shippingAddresses, Utils.SearchType.Name);

                                if (toBeSaved.Count != 0)
                                {
                                    Console.Write("Trying to save " + toBeSaved.Count + " shipping addresses to DB... ");
                                    int count = DBManager.SaveShippingAddressesToDB(toBeSaved);
                                    Console.WriteLine("Success! " + count + " have been saved!");
                                }
                                else
                                    Console.WriteLine("No customers were found for these addresses! Please import them before!");

                                return true;
                            }
                            else
                            {
                                Console.WriteLine("Type does not exist!");
                            }

                        }
                        else
                        {
                            Console.WriteLine("StartLine must be a number !");
                        }

                    }
                    else
                    {
                        Console.WriteLine("File does not exist!");
                    }

                    return false;

                }
                else
                {
                    Help();
                    return false;
                }

            }

            return true;
        }

        private static void Help()
        {
            Console.WriteLine("Syntax: CustomerManager.exe [dataSource | ? | help] [start | import | reset | delete] [path | id] [options]");
            Console.WriteLine("    ? or help:     Show help");
            Console.WriteLine("    dataSoure:     SQL Server");
            Console.WriteLine("    start:         Only start app");
            Console.WriteLine("    import:        Import infos from .csv file : import [path] [customer | address] [startLine]");
            Console.WriteLine("    delete:        Delete user & shipping addresses by id : delete [id]");
            Console.WriteLine("    reset:         Reset SQL Database");
        }



    }

}
