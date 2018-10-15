using CustomerManagement.Data;
using CustomerManager.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManagerApp.Data
{
    public class CustomerData
    {

        public static List<Customer> Customers { get; set; }

        public static void Initialize()
        {
            Customers = DbManager.LoadData();
        }

        public static void Clear()
        {
            Customers.Clear();
        }

        public static bool Contains(Customer customer) => DataManager.Contains(Customers, customer);

        public static void AddWithoutDoubles(List<Customer> customers) => DataManager.AddWithoutDoubles(Customers, customers);

        public static ShippingAddress FindShippingAddress(int id) => DataManager.FindShippingAddress(Customers, id);

        public static List<ShippingAddress> GetShippingAddresses() => DataManager.GetShippingAddresses(Customers);

        public static void Add(Customer customer)
        {
            if (!Contains(customer))
                Customers.Add(customer);
        }

        public static void Add(List<Customer> customers)
        {
            Customers.AddRange(customers);
        }

    }
}
