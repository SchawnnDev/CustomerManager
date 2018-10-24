using CustomerManagement.Data;
using CustomerManagement.Enums;
using CustomerManager.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CustomerManagement.Utils
{
    public class Utils
    {
        public static List<ShippingAddress> SearchShippingAddressesForCustomer(Customer customer, List<ShippingAddress> shippingAddresses, SearchType searchType)
        {

           var addresses = new List<ShippingAddress>();

            foreach (var address in shippingAddresses) // if statement better in or out of the loop?
            {
                var type = searchType;

                if (type == SearchType.Variable) type = FindCorrectSearchType(customer, address);
                switch (type)
                {
                    // Better switch?
                    case SearchType.Id when address.CustomerId == customer.Id:
                    // Admitting that no other has the same name&firstname
                    case SearchType.Name when address.FirstName.Equals(customer.FirstName) && address.Name.Equals(customer.Name):
                        addresses.Add(address);
                        break;
                }
            }

            return addresses;

        }

        public static void SearchShippingAddressesForCustomers(List<Customer> customers, List<ShippingAddress> shippingAddresses, SearchType type)
        {

            foreach (var customer in customers)
                customer.ShippingAddresses.AddRange(SearchShippingAddressesForCustomer(customer, shippingAddresses, type));

        }

        public static List<ShippingAddress> SearchCustomersForShippingAddresses(List<Customer> customers,List<ShippingAddress> shippingAddresses, SearchType type)
        {
            var addresses = new List<ShippingAddress>();

            foreach (var shippingAddress in shippingAddresses)
            {
                var address = DataManager.AddWithoutDoubles(type == SearchType.Id ? DataManager.Find(customers, shippingAddress.CustomerId) : DataManager.Find(customers, shippingAddress.Name, shippingAddress.FirstName), shippingAddress);
                if (address != null) addresses.Add(address);
            }
            return addresses;
        }

        public static SearchType FindCorrectSearchType(Customer customer, ShippingAddress address) => customer.Id != 0 && address.CustomerId != 0 ? SearchType.Id : SearchType.Name;

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsAlphanumeric(string str)
        {
            return !string.IsNullOrWhiteSpace(str) && Regex.IsMatch(str, @"^[a-zA-ZÀ-ž]+$");
        }

        public static bool DateEquals(DateTime date, DateTime other)
        {
            return date.Year == other.Year && date.Month == other.Month && date.Day == other.Day;
        }

    }

}
