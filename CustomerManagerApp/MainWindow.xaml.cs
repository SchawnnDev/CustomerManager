using CustomerManagement.Data;
using CustomerManagerApp.Data;
using CustomerManagerApp.Graphics.Models;
using CustomerManagerApp.Graphics.Windows;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace CustomerManagerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private CustomerListModel Model => DataContext as CustomerListModel;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new CustomerListModel();

            new ConnectionWindow(this, Settings.Default.DataSource, true).ShowDialog();

        }

        public void LoadList()
        {
            Model.Customers = new ObservableCollection<Customer>(CustomerData.Customers);
        }

        private void MenuSettings_Click(object sender, RoutedEventArgs e)
        {

            new ConnectionWindow(this, Settings.Default.DataSource, false).ShowDialog();

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("© Paul MEYER - AMC Datensysteme GmbH");
        }

        private void QuitApp_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ImportCustomer_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV Files|*.csv",
                Title = "Select a CSV File"
            };

            if (openFileDialog.ShowDialog() != true) return;

            new QuestionBox(openFileDialog.FileName, false).ShowDialog();

        }

        private void ImportShippingAddress_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV Files|*.csv",
                Title = "Select a CSV File",
                CheckFileExists = true
            };

            if (openFileDialog.ShowDialog() != true) return;

            new QuestionBox(openFileDialog.FileName, true).ShowDialog();

        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show($"Do you really want to reset the Database ?", "Reset Database", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

            if (result != MessageBoxResult.Yes) return;

            if (DbManager.Reset())
            {
                MessageBox.Show("Successfully resetted DataBase.", "Reset");
                CustomerData.Clear();
                return;
            }

            MessageBox.Show("Error occurred during reset", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

        }

        public void AddCustomer(Customer customer)
        {
            Model.Customers.Add(customer);
        }

        private void DisplayShippingAddresses_Click(object sender, RoutedEventArgs e)
        {
            var cust = GetCustomerFromSender(sender);

            if (cust == null) return;

            new DisplayShippingAddresses(cust, this).ShowDialog();

        }

        private void EditCustomer_Click(object sender, RoutedEventArgs e)
        {
            var cust = GetCustomerFromSender(sender);

            if (cust == null) return;

            new ManageCustomer(this, true, cust).ShowDialog();
        }

        private void DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            var customer = GetCustomerFromSender(sender);

            if (customer == null) return;

            MessageBoxResult result = MessageBox.Show($"Do you really want to delete customer { customer.FirstName } {customer.Name}?", "Delete Customer", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

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

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            new ManageCustomer(this, false, null).ShowDialog();
        }

        private void ExportShippingAddress_Click(object sender, RoutedEventArgs e)
        {
            new ExportShippingAddresses().ShowDialog();
        }

        private void ExportCustomer_Click(object sender, RoutedEventArgs e)
        {
            new ExportCustomers().ShowDialog();
        }

    }

}
