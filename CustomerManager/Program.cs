using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace CustomerManager
{
    class Program
    {

        public static List<Customer> Customers = new List<Customer>();

        static void Main(string[] args)
        {
            Console.WriteLine("Loading app CustomerManager...");

            DBManager.Init();

            DBManager.LoadData();

            string[] customers = ReadFile(args[0]).Skip(1).ToArray();
            
            foreach(string customer in customers)
            {
                string[] infos = customer.Split(',');
                Customers.Add(new Customer(infos[0], infos[1], DateTime.Parse(infos[2]), infos[3], infos[4]));
                
            }

        }

        private static string[] ReadFile(string path)
        {
            return File.ReadAllLines(path);
        }

    }

}
