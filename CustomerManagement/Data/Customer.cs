using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CustomerManagement.Data
{
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public List<ShippingAddress> ShippingAddresses { get; }

        public Customer()
        {
            ShippingAddresses = new List<ShippingAddress>();
        }

        public Customer(string firstName, string name, DateTime dateOfBirth, string phoneNumber, string email)
        {
            FirstName = firstName;
            Name = name;
            DateOfBirth = dateOfBirth;
            PhoneNumber = phoneNumber;
            Email = email;
            ShippingAddresses = new List<ShippingAddress>();
        }

        public void AddShippingAddress(ShippingAddress address)
        {
            if (DataManager.Contains(address, this)) return;
            ShippingAddresses.Add(address);
            address.Id = ShippingAddresses.Count;

        }

        public override bool Equals(object obj)
        {
            var customer = obj as Customer;
            return customer != null && ((Id != 0) ? Id == customer.Id : (FirstName == customer.FirstName && Name == customer.Name && Utils.Utils.DateEquals(DateOfBirth, customer.DateOfBirth)));
        }

        public string[] ToArray()
        {
            return new string[]
            {
                Id.ToString(),
                FirstName,
                Name,
                DateOfBirth.ToShortDateString(),
                PhoneNumber,
                Email
            };
        }

        public override string ToString()
        {
            return $"[{Id}] {FirstName} {Name}: {DateOfBirth.ToShortDateString()} | {PhoneNumber} | {Email}";
        }

        public bool HasShippingAddresses()
        {
            return ShippingAddresses.Count != 0;
        }

    }

}
