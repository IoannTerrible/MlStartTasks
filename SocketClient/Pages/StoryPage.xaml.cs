using Accessibility;
using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        private bool receivingLines = false;
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
            if (!receivingLines)
            {
                GoButton.Content = "Stop";
                try
                {
                    receivingLines = true;
                    ; 
                    await _mainWindow.SendMessageAndReceive("LOR");
                    receivingLines = false;
                }
                catch (Exception ex)
                {
                    Logger.LogByTemplate(Serilog.Events.LogEventLevel.Error, ex);
                }
            }
            else
            {
                GoButton.Content = "Go";
                _mainWindow.SendMessageAndReceive("LOR");
                receivingLines = false;
                MessageBox.Show("Retrieving storyLines has stopped");
            }
        }
    }
}
