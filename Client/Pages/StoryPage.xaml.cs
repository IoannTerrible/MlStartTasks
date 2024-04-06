using ClassLibrary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace SocketClient
{
    /// <summary>
    /// Логика взаимодействия для StoryPage.xaml
    /// </summary>
    public partial class StoryPage : Page
    {
        internal List<string> lines;
        internal float delay;
        private App _app;
        private MainWindow _mainWindow;
        public bool receivingLines = false;
        public StoryPage(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            InitializeComponent();
            _app = (App)Application.Current;
            this.Unloaded += StoryPage_Unloaded;

        }
        public async Task AddLineToListBoxWithDelay(string line)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                this.StoryListBox.Items.Add(line);
                var border = (Border)VisualTreeHelper.GetChild(StoryListBox, 0);
                var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.ScrollToEnd();
            }, DispatcherPriority.Background);
        }
        private async void StoryPage_Unloaded(object sender, RoutedEventArgs e)
        {
            receivingLines = false;
        }

        private async void Go_Click(object sender, RoutedEventArgs e)
        {
        }

        public void MakeGoToStop(bool isGo)
        {
            GoButton.Content = isGo ? "Go" : "Stop";
        }
    }
}
