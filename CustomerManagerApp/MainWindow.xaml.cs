using CustomerManagement.Data;
using CustomerManager;
using CustomerManager.Data;
using CustomerManagerApp.Graphics;
using CustomerManagerApp.Graphics.Models;
using CustomerManagerApp.Graphics.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            //Program.Test(new string[] { "t", "start" });
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
            openFileDialog.ShowDialog();
        }

        private void ImportShippingAddress_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV Files|*.csv",
                Title = "Select a CSV File"
            };
            openFileDialog.ShowDialog();
        }

    }
}
