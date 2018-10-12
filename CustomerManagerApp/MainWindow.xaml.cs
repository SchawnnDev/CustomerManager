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
            DbManager.DataSource = @"GRIEVOUS\HISTORIAN";
            DbManager.Init();
            DataManager.Init();
            DbManager.LoadData();
        }


        private void MenuSettings_Click(object sender, RoutedEventArgs e)
        {

            Settings settings = new Settings();
            settings.Activate();
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
