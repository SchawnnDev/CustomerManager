using CustomerManagement.Data;
using CustomerManagement.Enums;
using CustomerManagement.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CustomerManager.Data
{
    public class FileManager
    {

        private static bool IsValid(string line, int normalLength)
        {
            return line != null && !line.All(ch => ch.Equals(' ')) && Regex.Matches(line, ",").Count == normalLength;
        }

        public static List<Customer> ImportCustomers(string path, int startLine)
        {

            List<Customer> customers = new List<Customer>();

            Console.Write($"Searching customers in { path }... ");

            string[] file = ReadFile(path);
            if (startLine > file.Length) return customers;

            foreach (string customer in file.Skip(startLine - 1).ToArray())
            {
                if (!IsValid(customer, 4)) continue;
                string[] infos = customer.Split(',');
                if (infos.Length >= 5)
                    customers.Add(new Customer(infos[0], infos[1], DateTime.Parse(infos[2]), infos[3], infos[4]));
            }

            Console.WriteLine($"{customers.Count} found!");
            return customers;
        }

        public static List<ShippingAddress> ImportShippingAddress(string path, int startLine)
        {

            List<ShippingAddress> shippingAddresses = new List<ShippingAddress>();

            Console.Write($"Searching shipping addresses in {path}... ");

            string[] file = ReadFile(path);
            if (startLine > file.Length) return shippingAddresses;

            foreach (string address in file.Skip(startLine - 1).ToArray())
            {
                if (!IsValid(address, 3)) continue;
                string[] infos = address.Split(',');
                if (infos.Length >= 4)
                    shippingAddresses.Add(new ShippingAddress(infos[0], infos[1], infos[2], infos[3]));
            }

            Console.WriteLine($"{shippingAddresses.Count} found!");
            return shippingAddresses;
        }

        public static int ExportCustomers(string path, List<Customer> customers, ExportSettings[] settings)
        {
            var count = 0;


            using (var file = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                file.SetLength(0);

                using (var stream = new StreamWriter(file))
                {

                    // Write first info line
                    stream.WriteLine(StringUtils.BuildExportIntroductionLine(settings));

                    foreach (var customer in customers)
                    {
                        stream.WriteLine(StringUtils.BuildExportLine(customer, null, settings));
                        count++;
                    }

                }

            }

            return count;

        }

        public static int ExportShippingAddresses(string path, List<Customer> customers, ExportSettings[] settings)
        {
            var count = 0;

            using (var file = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                file.SetLength(0);

                using (var stream = new StreamWriter(file))
                {

                    // Write first info line
                    stream.WriteLine(StringUtils.BuildExportIntroductionLine(settings));

                    foreach (var customer in customers)
                    {

                        foreach (var address in customer.ShippingAddresses)
                        {
                            stream.WriteLine(StringUtils.BuildExportLine(customer, address, settings));
                            count++;
                        }

                    }

                }

            }

            return count;

        }

        private static string[] ReadFile(string path)
        {
            return File.ReadAllLines(path);
        }

    }

}
