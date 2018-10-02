using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Customer(string firstName, string name, DateTime dateOfBirth, string phoneNumber, string email)
        {
            FirstName = firstName;
            Name = name;
            DateOfBirth = dateOfBirth;
            PhoneNumber = phoneNumber;
            Email = email;
        }
    }

    class CustomerShippingAddress
    {

        public int CustomerId { get; set; }
        public string Address { get; private set; }
        public string PostalCode { get; private set; }

        public CustomerShippingAddress(int customerId, string address, string postalCode)
        {
            CustomerId = customerId;
            Address = address;
            PostalCode = postalCode;
        }
    }

}
