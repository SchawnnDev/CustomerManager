using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomerManagement.Data;
using CustomerManagement.Enums;

namespace CustomerManagement.Interfaces
{
    public interface IPlugin
    {

        string GetName();

        string GetDataSource();

        void SetDataSource(string dataSource);

        void Init();

        bool Reset();

        void UpdateCustomer(Customer customer);

        void UpdateShippingAddress(ShippingAddress address);

        void DeleteShippingAddress(int id);

        void DeleteCustomer(int id);

        Customer GetCustomer(int id);

        List<ShippingAddress> GetShippingAddresses(int customerId);

        List<Customer> LoadData();

        int SaveShippingAddressesToDb(List<ShippingAddress> shippingAddresses);

        int SaveCustomersToDb(List<Customer> customers);

    }
}
