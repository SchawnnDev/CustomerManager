using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManager.Data
{
    class DataManager
    {

        public static List<Customer> Customers { get; private set; }

        public static void Init()
        {
            Customers = new List<Customer>();
        }

        public static void Add(List<Customer> customers)
        {
            Customers.AddRange( customers );
        }

        public static void AddWithoutDoubles(List<Customer> customers)
        {
            foreach (Customer customer in customers)
                if (!Contains(customer))
                    Customers.Add(customer);
        }
        public static ShippingAddress AddWithoutDoubles(Customer customer, ShippingAddress shippingAddress)
        {

            if (customer == null || shippingAddress == null) return null;

            shippingAddress.CustomerId = customer.Id;

            if (customer.ShippingAddresses.Count == 0)
            {
                Customers.Add(customer);
                return shippingAddress;
            }

            foreach (ShippingAddress address in customer.ShippingAddresses)
            {
                if (shippingAddress.Address != address.Address && shippingAddress.PostalCode != address.PostalCode)
                {
                    Customers.Add(customer);
                    return address;
                }
            }
            return null;
        }

        public static bool Contains(Customer customer)
        {
            foreach (Customer cu in Customers)
                if (customer.Equals(cu)) return true;
            return false;
        }

        public static bool Contains(int id)
        {
            foreach (Customer customer in Customers)
                if (customer.Id != 0 && customer.Id == id) return true;
            return false;
        }
        public static Customer Find(int id)
        {
            foreach (Customer customer in Customers)
                if (customer.Id != 0 && customer.Id == id) return customer;
            return null;
        }

        public static Customer Find(string name, string firstName)
        {
            foreach (Customer customer in Customers)
                if (customer.Name.Equals(name) && customer.FirstName.Equals(firstName)) return customer;
            return null;
        }

    }
}
