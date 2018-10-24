using CustomerManagement.Data;
using CustomerManagement.Enums;
using CustomerManagement.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CustomerManagement.IO
{
    public class FileManager
    {

        private static bool IsValid(string line, int normalLength)
        {
            return line != null && !line.All(ch => ch.Equals(' ')) && Regex.Matches(line, ",").Count == normalLength;
        }

        public static List<Customer> ImportCustomers(string path, int startLine)
        {

           var customers = new List<Customer>();

            Console.Write($"Searching customers in { path }... ");

            var file = ReadFile(path);
            if (startLine > file.Length) return customers;

            customers.AddRange(file.Skip(startLine - 1)
                .ToArray()
                .Where(customer => IsValid(customer, 4))
                .Select(customer => customer.Split(','))
                .Where(infos => infos.Length >= 5)
                .Select(infos => new Customer(infos[0], infos[1], DateTime.Parse(infos[2]), infos[3], infos[4])));

            Console.WriteLine($"{customers.Count} found!");
            return customers;
        }

        public static List<ShippingAddress> ImportShippingAddress(string path, int startLine)
        {

            var shippingAddresses = new List<ShippingAddress>();

            Console.Write($"Searching shipping addresses in {path}... ");

            var file = ReadFile(path);
            if (startLine > file.Length) return shippingAddresses;

            shippingAddresses.AddRange(file.Skip(startLine - 1)
                .ToArray()
                .Where(address => IsValid(address, 3))
                .Select(address => address.Split(','))
                .Where(infos => infos.Length >= 4)
                .Select(infos => new ShippingAddress(infos[0], infos[1], infos[2], infos[3])));

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
