using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CustomerManager.Data
{
    class FileManager
    {

        private static bool IsValid(string line, int normalLength)
        {
            return line != null && !line.All(ch => ch.Equals(' ')) && Regex.Matches(line, ",").Count == normalLength;
        }

        public static List<Customer> ImportCustomers(string path, int startLine)
        {

            List<Customer> customers = new List<Customer>();

            Console.Write("Searching customers in " + path + "... ");

            foreach (string customer in ReadFile(path).Skip(startLine).ToArray())
            {
                if (!IsValid(customer, 4)) continue;
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

            Console.Write("Searching shipping addresses in " + path + "... ");

            foreach (string address in ReadFile(path).Skip(startLine).ToArray())
            {
                if (!IsValid(address, 3)) continue;
                string[] infos = address.Split(',');
                if (infos.Length >= 4)
                    shippingAddresses.Add(new ShippingAddress(infos[0], infos[1], infos[2], infos[3]));
            }

            Console.WriteLine(shippingAddresses.Count + " found!");
            return shippingAddresses;
        }

        private static string[] ReadFile(string path)
        {
            return File.ReadAllLines(path);
        }

    }
}
