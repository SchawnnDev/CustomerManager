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
        public bool WindowIsOpen { get; set; }


        public DisplayShippingAddresses(Customer customer)
        {
            InitializeComponent();
            DataContext = new ShippingAddressListModel();
            Dispatcher.Invoke(() => Model.ShippingAddresses = new ObservableCollection<ShippingAddress>(customer.ShippingAddresses));
        }

        private void EditAddress_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteAddress_Click(object sender, RoutedEventArgs e)
        {

        }

        private ShippingAddress GetCustomerFromSender(object sender)
        {
            var menuItem = (MenuItem)sender;

            var contextMenu = (ContextMenu)menuItem.Parent;

            var item = (DataGrid)contextMenu.PlacementTarget;

            return item.SelectedCells.Count == 0 ? null : (ShippingAddress)item.SelectedCells[0].Item;
        }

    }
}
