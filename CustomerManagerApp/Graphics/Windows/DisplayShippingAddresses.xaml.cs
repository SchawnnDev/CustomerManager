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
        private CustomerList List { get; }


        public DisplayShippingAddresses(Customer customer, CustomerList list)
        {
            InitializeComponent();
            DataContext = new ShippingAddressListModel();
            List = list;
            Dispatcher.Invoke(() => Model.ShippingAddresses = new ObservableCollection<ShippingAddress>(customer.ShippingAddresses));
            this.Closing += new System.ComponentModel.CancelEventHandler(Window_Closing);
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


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            List.WindowIsOpen = false;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {

        }

        private void QuitApp_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
