using CustomerManagement.Data;
using CustomerManagement.Utils;
using CustomerManager.Data;
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

namespace CustomerManagerApp.Graphics.Windows
{
    /// <summary>
    /// Interaction logic for ManageCustomer.xaml
    /// </summary>
    public partial class ManageCustomer : Window
    {

        private bool CancelClose { get; set; }
        private bool Editing { get; }
        private Customer Customer { get; set; }
        private MainWindow Main { get; }

        public ManageCustomer(MainWindow main, bool editing, Customer customer)
        {
            InitializeComponent();

            Editing = editing;
            CancelClose = true;
            Customer = customer;
            Main = main;

            if (editing)
            {
                Title = $"Editing Customer: {customer.FirstName} {customer.Name}";
                CreateButton.Content = "Save";
                Customer_FirstName.Text = customer.FirstName;
                Customer_Name.Text = customer.Name;
                Customer_DateOfBirth.SelectedDate = customer.DateOfBirth;
                Customer_Phone.Text = customer.PhoneNumber;
                Customer_Email.Text = customer.Email;
            }

            this.Closing += new System.ComponentModel.CancelEventHandler(Window_Closing);

        }

        private void DateOfBirth_Loaded(object sender, RoutedEventArgs e)
        {
            DatePicker cal = sender as DatePicker;
            cal.BlackoutDates.Add(new CalendarDateRange(DateTime.Now.AddDays(1), DateTime.MaxValue));
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (Check(Customer_FirstName) || Check(Customer_Name) || string.IsNullOrWhiteSpace(Customer_Phone.Text) || Check(Customer_Email)) return;
            DateTime dateOfBirth = Customer_DateOfBirth.DisplayDate;
            if (dateOfBirth == null)
            {
                Customer_DateOfBirth.BorderBrush = Brushes.Red;
                SystemSounds.Beep.Play();
                return;
            }

            if (Customer == null) Customer = new Customer();

            Customer.FirstName = Customer_FirstName.Text;
            Customer.Name = Customer_Name.Text;
            Customer.PhoneNumber = Customer_Phone.Text;
            Customer.Email = Customer_Email.Text;
            Customer.DateOfBirth = Customer_DateOfBirth.DisplayDate;

            if (Editing)
            {
                DbManager.UpdateCustomer(Customer);
            }
            else
            {

                List<Customer> cust = new List<Customer>() { Customer };

                if (CustomerData.Contains(Customer) || DbManager.SaveCustomersToDB(cust) == 0)
                {
                    MessageBox.Show("This customer is already registred in the database.", "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                CustomerData.Add(cust);
                Main.AddCustomer(Customer);

            }

            Main.CustomersGrid.Items.Refresh();
            CancelClose = false;
            this.Close();

            MessageBox.Show($"Successfully saved Customer to database.", "Confirmation");

        }

        private bool Check(TextBox box)
        {
            string str = box.Text;
            if ((box.Name == "Customer_Email" && Utils.IsValidEmail(str)) || (box.Name != "Customer_Email" && Utils.IsAlphanumeric(str))) return false;
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
