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
        public bool WindowIsOpen { get; set; }

        public CustomerList()
        {
            InitializeComponent();
            DataContext = new CustomerListModel();
            Dispatcher.Invoke(() => Model.Customers = new ObservableCollection<Customer>(DataManager.Customers));
            WindowIsOpen = false;
        }

        public void AddCustomer(Customer customer)
        {
            Model.Customers.Add(customer);
        }

        private void DisplayShippingAddresses_Click(object sender, RoutedEventArgs e)
        {
            var cust = GetCustomerFromSender(sender);

            if (cust == null || CheckWindowIsOpen()) return;

            DisplayShippingAddresses displayShippingAddresses = new DisplayShippingAddresses(cust, this);
            WindowIsOpen = true;
            displayShippingAddresses.Show();

        }

        private void EditCustomer_Click(object sender, RoutedEventArgs e)
        {
            var cust = GetCustomerFromSender(sender);

            if (cust == null || CheckWindowIsOpen()) return;

            ManageCustomer customer = new ManageCustomer(this, true, cust);
            WindowIsOpen = true;
            customer.Show();
        }

        private void DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            var customer = GetCustomerFromSender(sender);

            if (customer == null) return;

            MessageBoxResult result = MessageBox.Show($"Do you really want to delete Customer { customer.FirstName } {customer.Name}", "Delete Customer", MessageBoxButton.YesNo,MessageBoxImage.Exclamation);

            if (result != MessageBoxResult.Yes) return;

            DbManager.DeleteCustomer(customer.Id);
            Model.Customers.Remove(customer);

        }

        private Customer GetCustomerFromSender(object sender)
        {
            var menuItem = (MenuItem)sender;

            var contextMenu = (ContextMenu)menuItem.Parent;

            var item = (DataGrid)contextMenu.PlacementTarget;

            if (item.SelectedCells.Count == 0) return null;

            return item.SelectedCells.Count == 0 ? null : (Customer)item.SelectedCells[0].Item;
        }

        private bool CheckWindowIsOpen()
        {

            if(WindowIsOpen)
            {
                MessageBox.Show("A window is already open!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }

            return false;
        }
        private void QuitApp_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (CheckWindowIsOpen()) return;
            ManageCustomer manageCustomer = new ManageCustomer(this, false, null);
            WindowIsOpen = true;
            manageCustomer.Show();
        }

    }
}
