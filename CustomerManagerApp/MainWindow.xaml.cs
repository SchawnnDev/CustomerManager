using CustomerManagement.Data;
using CustomerManager.Data;
using CustomerManagerApp.Graphics.Windows;
using Microsoft.Win32;
using System.Windows;

namespace CustomerManagerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Settings settings;
            if (Properties.Settings.Default.SettingsKey.Contains("DataSource"))
                settings = new Settings(Properties.Settings.Default["DataSource"].ToString(), true);

            else
                settings = new Settings(null, true);

            settings.Show();

        }


        private void MenuSettings_Click(object sender, RoutedEventArgs e)
        {

            Settings settings = new Settings(Properties.Settings.Default["DataSource"].ToString(), false);
            settings.Show();

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            CustomerList customerList = new CustomerList();
            customerList.Show();
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

            new QuestionBox(openFileDialog.FileName, false).Show();

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

            new QuestionBox(openFileDialog.FileName, true).Show();

        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show($"Do you really want to reset the Database ?", "Reset Database", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

            if (result != MessageBoxResult.Yes) return;

            if (DbManager.Reset())
            {
                MessageBox.Show("Successfully resetted DataBase.", "Reset");
                DataManager.Customers.Clear();
                return;
            }

            MessageBox.Show("Error occurred during reset", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

        }

    }
}
