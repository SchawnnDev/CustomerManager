using CustomerManagement.Data;
using CustomerManager.Data;
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
        private string Data { get; set; }
        private bool Start { get; }

        public Settings(string dataSource,bool start)
        {
            InitializeComponent();
            this.Closing += new System.ComponentModel.CancelEventHandler(Window_Closing);
            CancelClose = true;
            Data = dataSource;
            Start = start;

            if (dataSource != null)
                DataSource.Text = dataSource.ToString();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelClose = false;
            this.Close();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            string txt = DataSource.Text;

            if (!string.IsNullOrWhiteSpace(txt))
            {

                DbManager.DataSource = txt;
                Properties.Settings.Default["DataSource"] = txt;
                Properties.Settings.Default.Save();

                if(Start)
                {
                    DbManager.Init();
                    DataManager.Init();
                    DbManager.LoadData();
                }

                CancelClose = false;
                return;
            }

            DataSource.BorderBrush = Brushes.Red;
            SystemSounds.Beep.Play();


        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Data == null)
            {
                Application.Current.Shutdown();
                return;
            }
            e.Cancel = CancelClose;
        }

    }
}
