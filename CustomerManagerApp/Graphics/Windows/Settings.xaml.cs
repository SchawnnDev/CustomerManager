using CustomerManagement.Data;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Media;

namespace CustomerManagerApp.Graphics.Windows
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {

        private bool CancelClose { get; set; }

        public Settings()
        {
            InitializeComponent();
            this.Closing += new System.ComponentModel.CancelEventHandler(Window_Closing);
            CancelClose = true;
            dataSource.Text = DbManager.DataSource;
        /*    if (DBManager.IntegratedSecurity)
                securityYes.IsChecked = true;
            else
                securityNo.IsChecked = true; */
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelClose = false;
            this.Close();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            string txt = dataSource.Text;

            if (txt != null && !txt.All(ch => ch.Equals(' ')))
            {

                DbManager.DataSource = txt;
                //  DBManager.IntegratedSecurity = securityYes.IsChecked == true ? true : false;
                CancelClose = false;
            }
            else
            {
                dataSource.BorderBrush = Brushes.Red;
                SystemSounds.Beep.Play();
            }


        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = CancelClose;
        }

    }
}
