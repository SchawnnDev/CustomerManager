using CustomerManagement.Data;
using CustomerManagement.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManagement.Utils
{
    class StringUtils
    {
        public static string BuildExportLine(Customer customer, ShippingAddress address, ExportSettings[] settings)
        {
            List<String> builder = new List<string>();

            foreach (var setting in settings)
            {
                switch (setting)
                {
                    case ExportSettings.Id:
                        if(address != null)
                            builder.Add(address.Id.ToString());
                        else
                            builder.Add(customer.Id.ToString());
                        break;
                    case ExportSettings.Customer_Id:
                        builder.Add(customer.Id.ToString());
                        break;
                    case ExportSettings.First_Name:
                        builder.Add(customer.FirstName);
                        break;
                    case ExportSettings.Name:
                        builder.Add(customer.Name);
                        break;
                    case ExportSettings.Date_of_Birth:
                        builder.Add(customer.DateOfBirth.ToString("dd-MM-yyyy"));
                        break;
                    case ExportSettings.Phone_Number:
                        builder.Add(customer.PhoneNumber);
                        break;
                    case ExportSettings.Email:
                        builder.Add(customer.Email);
                        break;
                    case ExportSettings.Address:
                        if (address != null)
                            builder.Add(address.Address);
                        break;
                    case ExportSettings.Postal_Code:
                        if (address != null)
                            builder.Add(address.PostalCode);
                        break;
                }

            }

            return string.Join(",", builder.ToArray());
        }

        public static string BuildExportIntroductionLine(ExportSettings[] settings)
        {
            List<String> builder = new List<string>();

            foreach (var setting in settings)
                builder.Add(setting.ToString());

            return string.Join(",", builder.ToArray());
        }

    }
}
