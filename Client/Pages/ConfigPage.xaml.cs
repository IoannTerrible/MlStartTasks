using SocketClient;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace SocketClient
{
    /// <summary>
    /// Логика взаимодействия для ConfigPage.xaml
    /// </summary>
    public partial class ConfigPage : Page
    {
        private MainWindow _window;
        private Dictionary<string, string> _configData; // Основное хранилище данных
        private ObservableCollection<KeyValuePair<string, string>> _gridData; // Дополнительное хранилище для DataGrid

        public ConfigPage(MainWindow window)
        {
            _window = window;
            InitializeComponent();

            _configData = App.ContentFromConfig;
            _gridData = new ObservableCollection<KeyValuePair<string, string>>(_configData);

            dictionaryDataGrid.ItemsSource = _gridData; 
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            
            _configData.Clear();
            foreach (var item in _gridData)
            {
                _configData.Add(item.Key, item.Value);
            }

        }
    }
}

