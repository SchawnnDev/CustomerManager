using CustomerManagement.Data;
using CustomerManager.Data;
using CustomerManagerApp.Data;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CustomerManagement.IO;

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

        public ConnectionWindow(MainWindow main, string dataSource, bool start)
        {
            InitializeComponent();
            this.Closing += Window_Closing;
            CancelClose = true;
            Data = dataSource;
            Start = start;
            Main = main;

            DatabaseTypeBox.ItemsSource = PluginManager.GetPluginNames();

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
            var txt = DataSource.Text;

            if (!string.IsNullOrWhiteSpace(txt))
            {

                PluginManager.GetActivePlugin().SetDataSource(txt);
                // Settings
                Settings.Default.DataSource = txt;
                Settings.Default.Save();
                // Init app
                Task.Run(() => Initialize());

                return;
            }

            DataSource.BorderBrush = Brushes.Red;
            SystemSounds.Beep.Play();

        }

        private void Initialize()
        {

            Dispatcher.Invoke(() =>
            {
                ConnectButton.IsEnabled = false;
                CancelButton.IsEnabled = false;
                Cursor = Cursors.Wait;
            });

            PluginManager.GetActivePlugin().Init();
            CustomerData.Initialize();

            Dispatcher.Invoke(() =>
            {
                Main.LoadList();
                CancelClose = false;
                Cursor = Cursors.Arrow;
                Close();
            });

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

        private void BrowseButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
