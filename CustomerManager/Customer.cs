using System;
using System.Collections.Generic;

namespace CustomerManager
{
    class Customer
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

        public override string ToString()
        {
            return "[" + Id + "] " + FirstName + " " + Name + ": " + DateOfBirth.ToShortDateString() + " | " + PhoneNumber + " | " + Email;
        }
    }

    class ShippingAddress
    {

        public int CustomerId { get; set; }
        public string FirstName { get; private set; }
        public string Name { get; private set; }
        public string Address { get; private set; }
        public string PostalCode { get; private set; }

        public ShippingAddress(int customerId, string address, string postalCode)
        {
            CustomerId = customerId;
            Address = address;
            PostalCode = postalCode;
        }

        public ShippingAddress(string firstName, string name, string address, string postalCode) : this(0, address, postalCode)
        {
            FirstName = firstName;
            Name = name;
        }

        public override string ToString()
        {
            return "[" + CustomerId + "] " + Address + " | " + PostalCode;
        }

    }

}
