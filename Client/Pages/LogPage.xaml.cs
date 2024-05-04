using ClassLibrary;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для LogPage.xaml
    /// </summary>
    public partial class LogPage : Page
    {
        public ObservableCollection<LogEntry> LogEntries { get; } = new ObservableCollection<LogEntry>();

        private MainWindow _window;
        private Drawer localDrawerForLogPage;
        private readonly Canvas rectangleContainer = new();

        public LogPage(MainWindow window)
        {
            _window = window;
            InitializeComponent();

            Canvas canvas = FindName("canvas3") as Canvas;
            canvas.Children.Add(rectangleContainer);

            ListWithSqlResponce.MouseDoubleClick += ListWithSqlResponce_MouseDoubleClick;
            localDrawerForLogPage = new Drawer(rectangleContainer, ImageBoxThree, _window);
            RefreshLogEntries();
        }
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshLogEntries();
        }
        private async void ListWithSqlResponce_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListWithSqlResponce.SelectedItem is LogEntry selectedLogEntry)
            {
                try
                {
                    localDrawerForLogPage.ClearRectangles();
                    string directoryPath = Path.Combine(App.PathToDirectory, "KeyFrames");
                    string framePath = selectedLogEntry.FramePath.Trim();
                    string filePath = Path.Combine(directoryPath, framePath);
                    string[] metaDataParts = selectedLogEntry.MetaData.Split(',');
                    ObjectOnPhoto objectOnPhoto = new ObjectOnPhoto
                    {
                        XTopLeftCorner = double.Parse(metaDataParts[0]),
                        YTopLeftCorner = double.Parse(metaDataParts[1]),
                        XBottonRigtCorner = double.Parse(metaDataParts[2]),
                        YBottonRigtCorner = double.Parse(metaDataParts[3]),
                        Class_name = metaDataParts[4],
                        Class_id = int.Parse(metaDataParts[5])
                    };
                    await LoadImageAndCalculateScale(filePath);
                    localDrawerForLogPage.DrawBoundingBox(objectOnPhoto.XTopLeftCorner,
                        objectOnPhoto.YTopLeftCorner,
                        objectOnPhoto.XBottonRigtCorner,
                        objectOnPhoto.YBottonRigtCorner,
                        objectOnPhoto.Class_name,
                        objectOnPhoto.Class_id,
                        true);
                }
                catch (Exception ex)
                {

                }
            }
        }
        private async Task LoadImageAndCalculateScale(string filePath)
        {
            ImageBoxThree.Source = new BitmapImage(new Uri(filePath, UriKind.RelativeOrAbsolute));

            await Task.Delay(100);

            localDrawerForLogPage.CalculateScale();
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

}
