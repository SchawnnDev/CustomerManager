using CustomerManagement.Data;
using CustomerManagement.Enums;
using CustomerManagement.Utils;
using CustomerManagement.Data;
using CustomerManagerApp.Data;
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
    /// Interaction logic for QuestionBox.xaml
    /// </summary>
    public partial class QuestionBox : Window
    {

        private string Path { get; }
        private bool Addresses { get; }

        public QuestionBox(string path, bool addresses)
        {
            InitializeComponent();
            Path = path;
            Addresses = addresses;

            if (!addresses) return;

            Title = "Shipping Addresses";

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public bool IsValid(string answer)
        {
            return !string.IsNullOrWhiteSpace(answer);
        }

        private void Import(string answer, int startLine)
        {
            Dispatcher.Invoke(() =>
            {

                OkButton.IsEnabled = false;
                CancelButton.IsEnabled = false;
                Cursor = Cursors.Wait;

            });

            try
            {

                if (Addresses)
                {

                    var shippingAddresses = FileManager.ImportShippingAddress(Path, startLine);
                    Console.WriteLine($@"Imported {shippingAddresses.Count} shipping addresses from { Path }");
                    var toBeSaved = Utils.SearchCustomersForShippingAddresses(CustomerData.Customers, shippingAddresses, SearchType.Name);

                    if (toBeSaved.Count != 0)
                        MessageBox.Show($"Successfully saved {PluginManager.GetActivePlugin().SaveShippingAddresses(toBeSaved)} shipping address(es) to database.");
                    else
                        MessageBox.Show("No customers were found for these addresses! Please import them before!", "No customers found", MessageBoxButton.OK, MessageBoxImage.Error);

                }
                else
                {

                    var customers = FileManager.ImportCustomers(Path, startLine);

                    Console.WriteLine($@"Imported {customers.Count} customer(s) from { Path}");

                    CustomerData.AddWithoutDoubles(customers);

                    MessageBox.Show($"Successfully saved {PluginManager.GetActivePlugin().SaveCustomers(customers)} customer(s) to database.");

                }

            }
            catch (Exception e)
            {
                MessageBox.Show($"Error occurred: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Dispatcher.Invoke(() =>
            {
                Cursor = Cursors.Arrow;
                Close();
            });

        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            string answer = Response.Text;


            if (!IsValid(answer) || !int.TryParse(answer, out int startLine) || startLine < 0)
            {
                Response.BorderBrush = Brushes.Red;
                SystemSounds.Beep.Play();
                return;
            }

            Task.Run(() => Import(answer, startLine));

        }

    }

}
