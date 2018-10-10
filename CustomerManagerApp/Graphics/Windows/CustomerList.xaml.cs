using CustomerManagement.Data;
using CustomerManager.Data;
using CustomerManagerApp.Graphics.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace CustomerManagerApp.Graphics.Windows
{
    /// <summary>
    /// Interaction logic for CustomerList.xaml
    /// </summary>
    public partial class CustomerList : Window
    {

        private CustomerListModel Model => DataContext as CustomerListModel;

        public CustomerList()
        {
            InitializeComponent();
            DataContext = new CustomerListModel();
            Dispatcher.Invoke(() => Model.Customers = new ObservableCollection<Customer>(DataManager.Customers));
            Model.Customers.Add(new Customer("Julian", "Dobrinkat", DateTime.Now, "+49 721 16179-103", "Dobrinkat@amc-ds.de"));

        }

        private void DisplayShippingAddresses_Click(object sender, RoutedEventArgs e)
        {

            ManageCustomer manageCustomer = new ManageCustomer(false, null);
            manageCustomer.Show();

        }

        private void EditCustomer_Click(object sender, RoutedEventArgs e)
        {
            ManageCustomer customer = new ManageCustomer(true, GetCustomerFromSender(sender));
            customer.Show();
        }

        private void DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            var customer = GetCustomerFromSender(sender);

            MessageBoxResult result = MessageBox.Show($"Do you really want to delete Customer n°{customer.Id} ({ customer.FirstName } {customer.Name})", "Delete Customer", MessageBoxButton.YesNo);

            if (result != MessageBoxResult.Yes) return;

            DbManager.DeleteCustomer(customer.Id);
            Model.Customers.Remove(customer);

        }

        private Customer GetCustomerFromSender(object sender)
        {
            var menuItem = (MenuItem)sender;

            var contextMenu = (ContextMenu)menuItem.Parent;

            var item = (DataGrid)contextMenu.PlacementTarget;

            return (Customer)item.SelectedCells[0].Item;
        }

    }
}
