using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using CustomerManagement.Data;
using CustomerManagement.Interfaces;

namespace XMLPlugin
{
    public class XMLPlugin : IDatabasePlugin
    {
        private string DataSource { get; set; }

        public string GetName()
        {
            return "XML";
        }

        public string GetDataSource()
        {
            return DataSource;
        }

        public bool NeedsFile()
        {
            return true;
        }

        public string GetFileExtension()
        {
            return ".xml";
        }

        public void SetDataSource(string dataSource)
        {
            DataSource = dataSource;
        }

        public void Init()
        {
            if (File.Exists(DataSource)) return;

            SaveCustomersToFile(new List<Customer>());

        }

        public bool Reset()
        {
            try
            {
                SaveCustomersToFile(new List<Customer>());
            }
            catch
            {
                return false;
            }

            return true;
        }

        public void UpdateCustomer(Customer customer)
        {
            var customers = GetCustomers();

            var cust = DataManager.Find(customers, customer.Id);

            cust.FirstName = customer.FirstName;
            cust.Name = customer.Name;
            cust.DateOfBirth = customer.DateOfBirth;
            cust.PhoneNumber = customer.PhoneNumber;
            cust.Email = customer.Email;

            SaveCustomersToFile(customers);
        }

        public void UpdateShippingAddress(ShippingAddress shippingAddress)
        {
            var customers = GetCustomers();

            var address = DataManager.FindShippingAddress(customers, shippingAddress.Id);

            address.CustomerId = shippingAddress.CustomerId;
            address.Address = shippingAddress.Address;
            address.PostalCode = shippingAddress.PostalCode;

            SaveCustomersToFile(customers);
        }

        public void DeleteShippingAddress(int id)
        {
            var customers = GetCustomers();
            var customer = DataManager.Find(customers, id);

            if (customer == null) return;
            var address = DataManager.GetShippingAddress(customer.ShippingAddresses, id);

            if (address == null) return;

            customer.ShippingAddresses.Remove(address);

            SaveCustomersToFile(customers);
        }

        public void DeleteCustomer(int id)
        {
            var customers = GetCustomers();
            var customer = DataManager.Find(customers, id);

            if (customer == null) return;

            customers.Remove(customer);

            SaveCustomersToFile(customers);
        }

        public Customer GetCustomer(int id)
        {
            return DataManager.Find(GetCustomers(), id);
        }

        public List<ShippingAddress> GetShippingAddresses(int customerId)
        {
            return DataManager.GetShippingAddresses(GetCustomers(), customerId);
        }

        public List<Customer> GetCustomers()
        {
            List<Customer> customers;
            var serializer = new XmlSerializer(typeof(List<Customer>));

            using (var reader = new StreamReader(DataSource))
            {
                customers = (List<Customer>)serializer.Deserialize(reader);
                reader.Close();
            }

            return customers;
        }

        public int SaveShippingAddresses(List<ShippingAddress> shippingAddresses)
        {

            if (shippingAddresses.Count == 0) return 0;

            var customers = GetCustomers();
            var count = 0;

            foreach (var shippingAddress in shippingAddresses)
            {
                DataManager.Find(customers, shippingAddress.CustomerId)?.AddShippingAddress(shippingAddress);
                count++;
            }

            SaveCustomersToFile(customers);

            return count;

        }

        public int SaveCustomers(List<Customer> customers)
        {
            var list = GetCustomers();

            var count = 0;

            foreach (var customer in customers)
            {

                if (customer.Id != 0 || DataManager.Contains(list, customer)) continue;

                list.Add(customer);
                customer.Id = list.Count;
                count++;

            }

            SaveCustomersToFile(customers);

            return count;

        }

        private void SaveCustomersToFile(List<Customer> customers)
        {

            var serializer = new XmlSerializer(typeof(List<Customer>));

            try
            {
                File.WriteAllText(DataSource, "");
                using (var writer = new StreamWriter(DataSource, true))
                {
                    serializer.Serialize(writer, customers);
                    writer.Close();
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }

        }


    }

    /*
    [Serializable()]
    [System.Xml.Serialization.XmlRoot("CustomerList")]
    internal class CustomerList
    {
        [XmlArray("Customers")]
        [XmlArrayItem("Customer", typeof(Customer))]
        public List<Customer> Customers { get; set; }
    }
    */
}
