using ClassLibrary;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для LogPage.xaml
    /// </summary>
    public partial class LogPage : Page
    {
        private MainWindow _window;
        public ObservableCollection<LogEntry> LogEntries { get; } = new ObservableCollection<LogEntry>();
        public LogPage(MainWindow window)
        {
            _window = window;
            InitializeComponent();
        }
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshLogEntries();
        }
        private void RefreshLogEntries()
        {
            LogEntries.Clear();
            string logEvents = SqlCore.ReturnLogEventAsString(MainWindow.connectionString);
            string[] parts = logEvents.Split('\t');
            while (parts.Length >= 4)
            {
                LogEntries.Add(new LogEntry
                {
                    UserName = parts[0],
                    FileName = parts[1],
                    FramePath = parts[2],
                    MetaData = parts[3]
                });
                parts = parts.Skip(4).ToArray();
            }
            ListWithSqlResponce.ItemsSource = LogEntries;
        }
    }
    public class LogEntry
    {
        public string UserName { get; set; }
        public string FileName { get; set; }
        public string FramePath { get; set; }
        public string MetaData { get; set; }
    }
}
