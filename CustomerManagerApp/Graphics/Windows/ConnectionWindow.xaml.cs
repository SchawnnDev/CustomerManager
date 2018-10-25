using CustomerManagement.Data;
using CustomerManagement.Data;
using CustomerManagerApp.Data;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CustomerManagement.Interfaces;
using CustomerManagement.IO;
using Microsoft.Win32;
using System.IO;

namespace CustomerManagerApp.Graphics.Windows
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class ConnectionWindow : Window
    {

        private bool CancelClose { get; set; }
        private string Data { get; set; }
        private bool Start { get; set; }
        private MainWindow Main { get; }

        public ConnectionWindow(MainWindow main, bool start)
        {
            InitializeComponent();
            this.Closing += Window_Closing;
            CancelClose = true;
            ConfigFile.Load();
            var databaseType = ConfigFile.GetLastCorrectDatabaseType();
            var dataSource = Data = ConfigFile.GetValue($"DataSource_{databaseType}");
            Start = start;
            Main = main;

            DatabaseTypeBox.ItemsSource = PluginManager.GetPluginNames();

            if (!Start)
                ConnectButton.Content = "Save";

            if (!string.IsNullOrWhiteSpace(dataSource))
                DataSource.Text = dataSource;

            if (string.IsNullOrEmpty(databaseType)) return;

            DatabaseTypeBox.SelectedIndex = PluginManager.GetIndexFromName(databaseType);

            var plugin = PluginManager.GetPluginFromName(databaseType);

            if (plugin != null && plugin.NeedsFile())
                BrowseButton.IsEnabled = true;

        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelClose = false;
            this.Close();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var txt = DataSource.Text;
            var dataType = DatabaseTypeBox.SelectedValue.ToString();


            if (string.IsNullOrWhiteSpace(dataType) || !PluginManager.ChoosePlugin(dataType))
            {
                DatabaseTypeBox.BorderBrush = Brushes.Red;
                SystemSounds.Beep.Play();
                return;
            }


            if (!string.IsNullOrWhiteSpace(txt))
            {

                PluginManager.GetActivePlugin().SetDataSource(txt);

                // Settings
                ConfigFile.Write("DatabaseType", dataType);
                ConfigFile.Write($"DataSource_{dataType}", txt);
                ConfigFile.Save();
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
            CustomerData.Initialize(Main);

            Dispatcher.Invoke(() =>
            {
                Start = false;
                Main.LoadList();
                CancelClose = false;
                Cursor = Cursors.Arrow;
                Close();
            });

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Start)
            {
                Application.Current.Shutdown();
                return;
            }
            e.Cancel = CancelClose;
        }

        private void BrowseButton_OnClick(object sender, RoutedEventArgs e)
        {

            var extension = PluginManager.GetPluginFromName(DatabaseTypeBox.SelectedValue.ToString())
                .GetFileExtension();

            var openFileDialog = new SaveFileDialog
            {
                Title = "Select a destination",
                InitialDirectory = DataSource.Text,
                Filter = $"{extension.Substring(1).ToUpper()} Files|*{extension}"
            };

            if (openFileDialog.ShowDialog() != true) return;

            DataSource.Text = openFileDialog.FileName;

        }

        private void DatabaseTypeBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ComboBox box)) return;

            var name = box.SelectedValue.ToString();

            BrowseButton.IsEnabled = PluginManager.GetPluginFromName(name).NeedsFile();

            DataSource.Text = ConfigFile.GetValue($"DataSource_{ name }");

        }
    }
}
