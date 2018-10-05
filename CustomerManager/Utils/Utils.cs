using CustomerManager.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManager.Utils
{
    class Utils
    {
        public static List<ShippingAddress> SearchShippingAddressesForCustomer(Customer customer, List<ShippingAddress> shippingAddresses, SearchType searchType)
        {

            List<ShippingAddress> addresses = new List<ShippingAddress>();

            foreach (ShippingAddress address in shippingAddresses) // if statement better in or out of the loop?
            {
                SearchType type = searchType;

                if (type == SearchType.Variable) type = FindCorrectSearchType(customer, address);
                if (type == SearchType.Id && address.CustomerId == customer.Id) // Better switch?
                    addresses.Add(address);
                else if (type == SearchType.Name && address.FirstName.Equals(customer.FirstName) && address.Name.Equals(customer.Name))
                    addresses.Add(address); // Admitting that no other has the same name&firstname

            }

            return addresses;

        }

        public static void SearchShippingAddressesForCustomers(List<Customer> customers, List<ShippingAddress> shippingAddresses, SearchType type)
        {

            foreach (Customer customer in customers)
            {
                customer.ShippingAddresses.AddRange(SearchShippingAddressesForCustomer(customer, shippingAddresses, type));
            }

        }

        public static List<ShippingAddress> SearchCustomersForShippingAddresses(List<ShippingAddress> shippingAddresses, SearchType type)
        {
            List<ShippingAddress> addresses = new List<ShippingAddress>();

            foreach (ShippingAddress shippingAddress in shippingAddresses) { 
                ShippingAddress address = DataManager.AddWithoutDoubles(type == SearchType.Id ? DataManager.Find(shippingAddress.CustomerId) : DataManager.Find(shippingAddress.Name, shippingAddress.FirstName), shippingAddress);
                if (address != null) addresses.Add(address);
            }
            return addresses;
        }

        public static SearchType FindCorrectSearchType(Customer customer, ShippingAddress address)
        {
            return customer.Id != 0 && address.CustomerId != 0 ? SearchType.Id : SearchType.Name;
        }

    }

    enum SearchType
    {
        Id, Name, Variable
    }

}
