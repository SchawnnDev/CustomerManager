using CustomerManagement.Enums;
using CustomerManager.Data;
using CustomerManagerApp.Data;
using Microsoft.Win32;
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
    /// Interaction logic for ExportShippingAddresses.xaml
    /// </summary>
    public partial class ExportShippingAddresses : Window
    {
        public ExportShippingAddresses()
        {
            InitializeComponent();
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {

            var path = FilePath.Text;
            ExportSettings[] settings = GetExportSettings();
            if (!IsPathValid(path)) return;
            if (settings.Length == 0)
            {
                MessageBox.Show("You have unselected all export options.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Task.Run(() => Export(path, settings));

        }

        private void Export(string path, ExportSettings[] settings)
        {
            Dispatcher.Invoke(() =>
            {
                CreateButton.IsEnabled = false;
                CancelButton.IsEnabled = false;
                Cursor = Cursors.Wait;
            });

            try
            {
                int customerCount = FileManager.ExportShippingAddresses(path, CustomerData.Customers, settings);

                if (customerCount != 0)
                    MessageBox.Show($"Successfully saved {customerCount} shipping address(es) to {path}.");
                else
                    MessageBox.Show("No shipping addresses were found in the database.", "No shipping addresses found", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            }
            catch (Exception exception)
            {
                MessageBox.Show($"Error occurred: {exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Dispatcher.Invoke(() =>
            {
                Cursor = Cursors.Arrow;
                Close();
            });
        }

        private ExportSettings[] GetExportSettings()
        {
            List<ExportSettings> settings = new List<ExportSettings>();

            if (Check_ID.IsChecked.Value)
                settings.Add(ExportSettings.Id);
            if (Check_CustomerId.IsChecked.Value)
                settings.Add(ExportSettings.Customer_Id);
            if (Check_FirstName.IsChecked.Value)
                settings.Add(ExportSettings.First_Name);
            if (Check_Name.IsChecked.Value)
                settings.Add(ExportSettings.Name);
            if (Check_Address.IsChecked.Value)
                settings.Add(ExportSettings.Address);
            if (Check_PostalCode.IsChecked.Value)
                settings.Add(ExportSettings.Postal_Code);

            return settings.ToArray();

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Select a folder",
                CheckPathExists = true,
                Filter = "CSV Files|*.csv"
            };

            if (saveFileDialog.ShowDialog() != true) return;

            var path = saveFileDialog.FileName;

            if (IsPathValid(path))
                FilePath.Text = path;

        }


        private bool IsPathValid(string txt)
        {
            if (string.IsNullOrWhiteSpace(txt) || !txt.EndsWith(".csv"))
            {
                FilePath.BorderBrush = Brushes.Red;
                SystemSounds.Beep.Play();
                return false;
            }

            return true;
        }

    }

}
