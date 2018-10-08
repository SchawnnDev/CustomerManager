using System;
using System.Collections.Generic;

namespace CustomerManagement.Data
{
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; private set; }
        public string Name { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public string PhoneNumber { get; private set; }
        public string Email { get; private set; }
        public List<ShippingAddress> ShippingAddresses { get; }

        public Customer(string firstName, string name, DateTime dateOfBirth, string phoneNumber, string email)
        {
            FirstName = firstName;
            Name = name;
            DateOfBirth = dateOfBirth;
            PhoneNumber = phoneNumber;
            Email = email;
            ShippingAddresses = new List<ShippingAddress>();
        }

        public override bool Equals(object obj)
        {
            var customer = obj as Customer;
            return customer != null && (Id != 0) ? Id == customer.Id : (FirstName == customer.FirstName && Name == customer.Name);
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
