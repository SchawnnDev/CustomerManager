using CustomerManagement.Data;
using CustomerManagement.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomerManagement.IO;

namespace CustomerManagerApp.Data
{
    public class CustomerData
    {

        public static List<Customer> Customers { get; set; }
        private static MainWindow MainWindow { get; set; }

        public static void Initialize(MainWindow main)
        {
            Customers = PluginManager.GetActivePlugin().GetCustomers();
            MainWindow = main;
        }

        public static void Clear()
        {
            Customers.Clear();
        }

        public static bool Contains(Customer customer) => DataManager.Contains(Customers, customer);

        public static void AddWithoutDoubles(List<Customer> customers)
        {

            foreach (var customer in customers)
            {
                if(Contains(customer)) continue;
                Add(customer);
                MainWindow.AddCustomer(customer);
            }

        }

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
