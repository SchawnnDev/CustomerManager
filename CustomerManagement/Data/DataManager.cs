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

        public static bool Contains(List<Customer> customers, int id) => Find(customers, id) != null;

        public static Customer Find(List<Customer> customers,int id)
        {
            foreach (var customer in customers)
                if (customer.Id != 0 && customer.Id == id) return customer;
            return null;
        }

        public static Customer Find(List<Customer> customers, string name, string firstName)
        {
            foreach (var customer in customers)
                if (customer.Name.Equals(name) && customer.FirstName.Equals(firstName)) return customer;
            return null;
        }

        public static void Remove(List<Customer> customers, Customer customer)
        {
            if (!Contains(customers,customer)) return;
            customers.Remove(customer);
        }

        public static void AddWithoutDoubles(List<Customer> list, List<Customer> customers)
        {
            foreach (var customer in customers)
                if (!Contains(list, customer))
                    list.Add(customer);
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

        public static bool Contains(List<Customer> customers, Customer customer)
        {
            foreach (var cu in customers)
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

        public static bool Contains(List<Customer> customers, ShippingAddress shipping)
        {

            foreach (var customer in customers)
                foreach (var address in customer.ShippingAddresses)
                    if (customer.Id == shipping.CustomerId && shipping.PostalCode == address.PostalCode && shipping.Address == address.Address)
                        return true;

            return false;
        }


        public static ShippingAddress FindShippingAddress(List<Customer> customers, int id)
        {
            foreach (var shippingAddress in GetShippingAddresses(customers))
                if (shippingAddress.Id != 0 && shippingAddress.Id == id) return shippingAddress;
            return null;
        }

        public static List<ShippingAddress> GetShippingAddresses(List<Customer> customers)
        {
            List<ShippingAddress> shippingAddresses = new List<ShippingAddress>();

            foreach (var customer in customers)
                shippingAddresses.AddRange(customer.ShippingAddresses);

            return shippingAddresses;

        }

    }
}
