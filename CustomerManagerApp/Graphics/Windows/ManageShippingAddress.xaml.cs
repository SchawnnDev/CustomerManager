using CustomerManagement.Data;
using CustomerManagement.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CustomerManagement.IO;

namespace CustomerManagerApp.Graphics.Windows
{
    /// <summary>
    /// Interaction logic for ManageShippingAddress.xaml
    /// </summary>
    public partial class ManageShippingAddress : Window
    {

        private bool CancelClose { get; set; }
        private bool Editing { get; }
        private ShippingAddress Address { get; set; }
        private DisplayShippingAddresses DisplayShippingAddresses { get; }
        private Customer Customer { get; }

        public ManageShippingAddress(DisplayShippingAddresses displayShippingAddresses, bool editing, Customer customer, ShippingAddress address)
        {
            InitializeComponent();

            Editing = editing;
            CancelClose = true;
            Address = address ?? new ShippingAddress();
            DisplayShippingAddresses = displayShippingAddresses;
            Customer = customer;

            if (editing)
            {
                Title = $"Editing shipping address from {customer.FirstName} {customer.Name}";
                CreateButton.Content = "Save";
                ShippingAddressAddress.Text = Address.Address;
                ShippingAddressPostalCode.Text = Address.PostalCode;
            } else
            {
                Title = $"Create shipping address for {customer.FirstName} {customer.Name}";
            }

            this.Closing += new System.ComponentModel.CancelEventHandler(Window_Closing);

        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (Check(ShippingAddressAddress) || Check(ShippingAddressPostalCode)) return;

            Address.Address = ShippingAddressAddress.Text;
            Address.PostalCode = ShippingAddressPostalCode.Text;

            if (Editing)
            {
                 PluginManager.GetActivePlugin().UpdateShippingAddress(Address);
            }
            else
            {

                var ship = new List<ShippingAddress>() { Address };

                if (DataManager.Contains(Address,Customer) ||  PluginManager.GetActivePlugin().SaveShippingAddresses(ship) == 0)
                {
                    MessageBox.Show("This shipping address is already registred in the database.", "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                DataManager.AddWithoutDoubles(Customer, Address);
                DisplayShippingAddresses.AddShippingAddress(Address);

            }

            DisplayShippingAddresses.AddressGrid.Items.Refresh();
            CancelClose = false;
            this.Close();

            MessageBox.Show($"Successfully saved shipping address to DB", "Confirmation");

        }

        private bool Check(TextBox box)
        {
            if (!string.IsNullOrEmpty(box.Text)) return false;
            box.BorderBrush = Brushes.Red;
            SystemSounds.Beep.Play();
            return true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CancelClose = false;
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = CancelClose;
        }
    }
}

