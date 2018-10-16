using CustomerManagement.Data;
using CustomerManagement.Utils;
using CustomerManager.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManager
{
    class DisplayData
    {

        public static void Display(List<Customer> customers)
        {

            List<string[]> data = new List<string[]>
            {
                new string[] { "Id", "First Name", "Name", "Date of Birth", "Phone Number", "Email", "Address Id", "Postal Code", "Address" }
            };

            foreach (var customer in customers)
            {
                bool post = true;
                if (customer.HasShippingAddresses())
                {
                    if (customer.ShippingAddresses.Count != 1)
                    {
                        data.Add(AddToArray(customer.ToArray(), false, ArrowDown(), ArrowDown(), ArrowDown()));
                    }
                    else
                    {
                        data.Add(AddToArray(customer.ToArray(), false, customer.ShippingAddresses.First().ToArray()));
                        post = false;
                    }
                }
                else
                    data.Add(AddToArray(customer.ToArray(), false, Cross(), Cross(), Cross()));

                if (post)
                    foreach (var address in customer.ShippingAddresses)
                        data.Add(AddToArray(address.ToArray(), true, customer.Id.ToString(), ArrowDownRight(), ArrowDownRight(), ArrowDownRight(), ArrowDownRight(), ArrowDownRight()));

            }


            Console.WriteLine(data.Count != 1
                ? ArrayPrinter.GetDataInTableFormat(data)
                : "No customers & addresses in database.");
        }

        private static string[] AddToArray(string[] array, bool before, params string[] add)
        {
            List<string> list = new List<string>(array);

            if (before)
                list.InsertRange(0, add);
            else
                list.AddRange(add);

            return list.ToArray();
        }

        private static string ArrowDown()
        {
            return "\u2193 \u2193 \u2193";
        }

        private static string ArrowDownRight()
        {
            return "\u2192 \u2192 \u2192";
        }

        private static string Cross()
        {
            return "\u2573 \u2573 \u2573";
        }

    }
}
