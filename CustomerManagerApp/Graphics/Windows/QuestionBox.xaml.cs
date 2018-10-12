using CustomerManagement.Data;
using CustomerManagement.Utils;
using CustomerManager.Data;
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

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            string answer = Response.Text;


            if (!IsValid(answer) || !int.TryParse(answer, out int startLine) || startLine < 0)
            {
                Response.BorderBrush = Brushes.Red;
                SystemSounds.Beep.Play();
                return;
            }

            if (Addresses)
            {
                List<ShippingAddress> shippingAddresses = FileManager.ImportShippingAddress(Path, startLine);
                Console.WriteLine($"Imported {shippingAddresses.Count} shipping addresses from { Path }");
                List<ShippingAddress> toBeSaved = Utils.SearchCustomersForShippingAddresses(shippingAddresses, SearchType.Name);

                if (toBeSaved.Count != 0)
                    MessageBox.Show($"Successfully saved {DbManager.SaveShippingAddressesToDB(toBeSaved)} Shipping Addresses to DB.");
                else
                    MessageBox.Show("No customers were found for these addresses! Please import them before!", "No customers found", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            else
            {

                List<Customer> customers = FileManager.ImportCustomers(Path, startLine);

                Console.WriteLine($"Imported {customers.Count} customers from { Path}");

                DataManager.AddWithoutDoubles(customers);

                MessageBox.Show($"Successfully saved {DbManager.SaveCustomersToDB(customers)} Customers to DB.");

            }

            Close();

        }

    }

}
