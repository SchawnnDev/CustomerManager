using CustomerManager.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManager.Help
{
    class Commands
    {

        private static List<ICommand> CommandList;

        public static void Initialize()
        {
            CommandList = new List<ICommand>();
            CommandList.Add(new Import());
        }

    }

    class Import : ICommand
    {
        // path , type
        public void Execute(string[] args)
        {
            if (args.Length <= 4)
            {
                Help();
            }
            else
            {
                string type = args[0].ToLower();
                string path = args[1];
                int startLine = Int32.Parse(args[2]);
                bool db = args[3].ToLower().Equals("yes");

                if (startLine < 0)
                    startLine = 0;

                if (File.Exists(path) && path.EndsWith(".csv"))
                {
                    if (type.Equals("customer"))
                    {
                        List<Customer> customers = FileManager.ImportCustomers(path, startLine);
                        DataManager.AddWithoutDoubles(customers);
                        if (db) DBManager.SaveCustomersToDB(customers);

                    }
                    else if (type.Equals("address"))
                    {

                    }
                    else
                    {
                        Console.WriteLine("TYPE DOESNT EXIST!!!");
                    }
                }
                else
                {
                    Console.WriteLine("PATH DOESNT EXIST OR WRONG FILE TYPE!!!");
                }
            }
        }

        public void Help()
        {
            Console.WriteLine("Syntax for import : import [type] [path] [startLine] [yes/no]");
            Console.WriteLine("[type] : customer | address");
            Console.WriteLine("[path] : existing path from .csv file");
            Console.WriteLine("[startLine] : where the data starts in file");
            Console.WriteLine("[yes/no] : directly write to db if user doesnt exist");
        }

        public string Name()
        {
            return "import";
        }
    }

}
