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
            //delay = float.Parse(App.ContentFromConfig[2]);

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
            //await Task.Delay(TimeSpan.FromSeconds(delay));
        }
        private async void StoryPage_Unloaded(object sender, RoutedEventArgs e)
        {
            receivingLines = false;
        }

        private async void Go_Click(object sender, RoutedEventArgs e)
        {
            //if (!receivingLines)
            //{
            //    MakeGoToStop(false);
            //    try
            //    {
            //        receivingLines = true; 
            //        await _mainWindow.SendMessageAndReceive("LOR");
            //        receivingLines = false;
            //        MakeGoToStop(true);
            //    }
            //    catch (Exception ex)
            //    {
            //        Logger.LogByTemplate(Serilog.Events.LogEventLevel.Error, ex);
            //    }
            //}
            //else
            //{
            //    MakeGoToStop(true);
            //    _mainWindow.SendMessageAndReceive("LOR");
            //    receivingLines = false;
            //    MessageBox.Show("Retrieving storyLines has stopped");
            //}
        }

        public void MakeGoToStop(bool isGo)
        {
            GoButton.Content = isGo ? "Go" : "Stop";
        }
    }
}
