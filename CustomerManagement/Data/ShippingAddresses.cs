using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManagement.Data
{
    public class ShippingAddress
    {

        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }

        public ShippingAddress()
        {

        }

        public ShippingAddress(int customerId)
        {
            CustomerId = customerId;
        }

        public ShippingAddress(int id, int customerId, string address, string postalCode)
        {
            Id = id;
            CustomerId = customerId;
            Address = address;
            PostalCode = postalCode;
        }

        public ShippingAddress(string firstName, string name, string address, string postalCode) : this(0, 0, address, postalCode)
        {
            FirstName = firstName;
            Name = name;
        }

        public string[] ToArray()
        {
            return new string[]
            {

                Id.ToString(),
                Address,
                PostalCode
            };
        }

        public override string ToString()
        {
            return $"[{ CustomerId }] {Address} | {PostalCode}";
        }

    }
}
