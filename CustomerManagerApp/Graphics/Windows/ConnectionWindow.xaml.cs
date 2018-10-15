using CustomerManagement.Data;
using CustomerManager.Data;
using CustomerManagerApp.Data;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Media;

namespace CustomerManagerApp.Graphics.Windows
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class ConnectionWindow : Window
    {

        private bool CancelClose { get; set; }
        private string Data { get; set; }
        private bool Start { get; }
        private MainWindow Main { get; }

        public ConnectionWindow(MainWindow main, string dataSource,bool start)
        {
            InitializeComponent();
            this.Closing += new System.ComponentModel.CancelEventHandler(Window_Closing);
            CancelClose = true;
            Data = dataSource;
            Start = start;
            Main = main;

            if (!Start)
                ConnectButton.Content = "Save";

            if (!string.IsNullOrWhiteSpace(dataSource))
                DataSource.Text = dataSource;
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
                // Settings
                Settings.Default.DataSource = txt;
                Settings.Default.Save();
                // Init app
                DbManager.Init();
                CustomerData.Initialize();
                Main.LoadList();
                //
                CancelClose = false;
                Close();
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
