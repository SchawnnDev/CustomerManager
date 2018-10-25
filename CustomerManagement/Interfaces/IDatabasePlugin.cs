using System.Collections.Generic;
using CustomerManagement.Data;

namespace CustomerManagement.Interfaces
{
    public interface IDatabasePlugin
    {

        string GetName();

        string GetDataSource();

        bool NeedsFile();

        string GetFileExtension();

        void SetDataSource(string dataSource);

        void Init();

        bool Reset();

        void UpdateCustomer(Customer customer);

        void UpdateShippingAddress(ShippingAddress address);

        void DeleteShippingAddress(int id);

        void DeleteCustomer(int id);

        Customer GetCustomer(int id);

        List<ShippingAddress> GetShippingAddresses(int customerId);

        List<Customer> GetCustomers();

        int SaveShippingAddresses(List<ShippingAddress> shippingAddresses);

        int SaveCustomers(List<Customer> customers);

    }
}
