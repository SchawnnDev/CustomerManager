using CustomerManagement.Data;
using CustomerManagement.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManager.Data
{
    public class DataManager
    {

        public static List<Customer> Customers { get; private set; }

        public static void Init()
        {
            Customers = new List<Customer>();
        }

        public static void Add(List<Customer> customers)
        {
            Customers.AddRange(customers);
        }

        public static void Remove(Customer customer)
        {
            if (!Contains(customer)) return;
            Customers.Remove(customer);
        }

        public static void AddWithoutDoubles(List<Customer> customers)
        {
            foreach (var customer in customers)
                if (!Contains(customer))
                    Customers.Add(customer);
        }
        public static ShippingAddress AddWithoutDoubles(Customer customer, ShippingAddress shippingAddress)
        {

            if (customer == null || shippingAddress == null) return null;

            shippingAddress.CustomerId = customer.Id;

            if (customer.Id == 0 || Contains(shippingAddress, customer))
            {
                return null;
            }

            customer.ShippingAddresses.Add(shippingAddress);
            return shippingAddress;

        }

        public static bool Contains(Customer customer)
        {
            foreach (var cu in Customers)
                if (customer.Equals(cu)) return true;
            return false;
        }

        public static bool Contains(ShippingAddress shipping, Customer customer)
        {

            foreach (var address in customer.ShippingAddresses)
                if (customer.Id == shipping.CustomerId && shipping.PostalCode == address.PostalCode && shipping.Address == address.Address)
                    return true;

            return false;
        }

        public static bool Contains(ShippingAddress shipping)
        {

            foreach (var customer in Customers)
                foreach (var address in customer.ShippingAddresses)
                    if (customer.Id == shipping.CustomerId && shipping.PostalCode == address.PostalCode && shipping.Address == address.Address)
                        return true;

            return false;
        }

        public static bool Contains(int id)
        {
            return Find(id) != null;
        }
        public static Customer Find(int id)
        {
            foreach (var customer in Customers)
                if (customer.Id != 0 && customer.Id == id) return customer;
            return null;
        }

        public static Customer Find(string name, string firstName)
        {
            foreach (var customer in Customers)
                if (customer.Name.Equals(name) && customer.FirstName.Equals(firstName)) return customer;
            return null;
        }


        public static ShippingAddress FindShippingAddress(int id)
        {
            foreach (var shippingAddress in GetShippingAddresses())
                if (shippingAddress.Id != 0 && shippingAddress.Id == id) return shippingAddress;
            return null;
        }

        public static List<ShippingAddress> GetShippingAddresses()
        {
            List<ShippingAddress> shippingAddresses = new List<ShippingAddress>();

            foreach (var customer in Customers)
                shippingAddresses.AddRange(customer.ShippingAddresses);

            return shippingAddresses;

        }

        public static void DisplayData()
        {

            List<string[]> data = new List<string[]>
            {
                new string[] { "Id", "First Name", "Name", "Date Of Birth", "Phone Number", "Email", "Address Id", "Postal Code", "Address" }
            };

            foreach (var customer in Customers)
            {
                bool post = true;
                if (customer.HasShippingAddresses())
                {
                    if (customer.ShippingAddresses.Count != 1)
                    {
                        data.Add(AddToArray(customer.ToArray(), false, ArrowDown(), ArrowDown(), ArrowDown()));
                    }
                    else
                    {
                        data.Add(AddToArray(customer.ToArray(), false, customer.ShippingAddresses.First().ToArray()));
                        post = false;
                    }
                }
                else
                    data.Add(AddToArray(customer.ToArray(), false, Cross(), Cross(), Cross()));

                if (post)
                    foreach (var address in customer.ShippingAddresses)
                        data.Add(AddToArray(address.ToArray(), true, customer.Id.ToString(), ArrowDownRight(), ArrowDownRight(), ArrowDownRight(), ArrowDownRight(), ArrowDownRight()));

            }


            if (data.Count != 1)
                Console.WriteLine(ArrayPrinter.GetDataInTableFormat(data));
            else
                Console.WriteLine("No customers & addresses in database.");

        }

        private static string[] AddToArray(string[] array, bool before, params string[] add)
        {
            List<string> list = new List<string>(array);

            if (before)
                list.InsertRange(0, add);
            else
                list.AddRange(add);

            return list.ToArray();
        }

        private static string ArrowDown()
        {
            return "\u2193 \u2193 \u2193";
        }

        private static string ArrowDownRight()
        {
            return "\u2192 \u2192 \u2192";
        }

        private static string Cross()
        {
            return "\u2573 \u2573 \u2573";
        }

    }
}
