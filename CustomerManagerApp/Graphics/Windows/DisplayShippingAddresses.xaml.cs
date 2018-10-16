using CustomerManagement.Data;
using CustomerManagerApp.Graphics.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace CustomerManagerApp.Graphics.Windows
{
    /// <summary>
    /// Interaction logic for DisplayShippingAddresses.xaml
    /// </summary>
    public partial class DisplayShippingAddresses : Window
    {

        private ShippingAddressListModel Model => DataContext as ShippingAddressListModel;
        private MainWindow Main { get; }
        private Customer Customer { get; }


        public DisplayShippingAddresses(Customer customer, MainWindow main)
        {
            InitializeComponent();
            DataContext = new ShippingAddressListModel();
            Main = main;
            Customer = customer;
            Dispatcher.Invoke(() => Model.ShippingAddresses = new ObservableCollection<ShippingAddress>(customer.ShippingAddresses));
        }

        private void EditAddress_Click(object sender, RoutedEventArgs e)
        {
            var ship = GetShippingAddressFromSender(sender);

            if (ship == null) return;

            ManageShippingAddress manageShippingAddress = new ManageShippingAddress(this, true, Customer, ship);
            manageShippingAddress.ShowDialog();
        }

        private void DeleteAddress_Click(object sender, RoutedEventArgs e)
        {
            var ship = GetShippingAddressFromSender(sender);

            if (ship == null) return;

            MessageBoxResult result = MessageBox.Show($"Do you really want to delete shipping address { ship.Address }", "Delete shipping address", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

            if (result != MessageBoxResult.Yes) return;

            DbManager.DeleteShippingAddress(ship.Id);
            Model.ShippingAddresses.Remove(ship);

        }

        public void AddShippingAddress(ShippingAddress address)
        {
            Model.ShippingAddresses.Add(address);
        }

        private ShippingAddress GetShippingAddressFromSender(object sender)
        {
            var menuItem = (MenuItem)sender;

            var contextMenu = (ContextMenu)menuItem.Parent;

            var item = (DataGrid)contextMenu.PlacementTarget;

            return item.SelectedCells.Count == 0 ? null : (ShippingAddress)item.SelectedCells[0].Item;
        }


        private void Create_Click(object sender, RoutedEventArgs e)
        {
            new ManageShippingAddress(this, false, Customer, null).ShowDialog();
        }

        private void QuitApp_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
