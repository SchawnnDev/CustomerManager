using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManager.Data
{
    class FileManager
    {

        public static List<Customer> ImportCustomers(string path, int startLine)
        {

            List<Customer> customers = new List<Customer>();

            Console.Write("Searching customers in " + path + " ");

            foreach (string customer in ReadFile(path).Skip(startLine).ToArray())
            {
                string[] infos = customer.Split(',');
                if (infos.Length >= 5)
                    customers.Add(new Customer(infos[0], infos[1], DateTime.Parse(infos[2]), infos[3], infos[4]));
            }

            Console.WriteLine(customers.Count + " found!");
            return customers;
        }

        public static List<ShippingAddress> ImportShippingAddress(string path, int startLine)
        {

            List<ShippingAddress> shippingAddresses = new List<ShippingAddress>();

            Console.Write("Searching shipping addresses in " + path + " ");

            foreach (string customer in ReadFile(path).Skip(startLine).ToArray())
            {
                string[] infos = customer.Split(',');
                if (infos.Length >= 5)
                    shippingAddresses.Add(new ShippingAddress(infos[0], infos[1], infos[2], infos[3]));
            }

            Console.Write(shippingAddresses.Count + " found!");
            return shippingAddresses;
        }

        public static string[] ReadFile(string path)
        {
            return File.ReadAllLines(path);
        }

    }
}
