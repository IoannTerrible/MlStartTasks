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
        internal float delays;
        private App _app;
        public StoryPage(MainWindow _mainWindow, List<string> stringsForLines, float delayForDelay)
        {
            InitializeComponent();
            _app = (App)Application.Current;
            lines = stringsForLines;
            delays = delayForDelay;
            _app.ProcessLinesInBackground(this);

        }
        public async void StartProcessingLines(List<string> storyLinesFromApp, float delayInSeconds)
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    foreach (var line in storyLinesFromApp)
                    {
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            this.StoryListBox.Items.Add(line);
                        }, DispatcherPriority.Background);
                        await Task.Delay((int)delayInSeconds / 1000);
                    }
                }
            });
        }
    }
}
