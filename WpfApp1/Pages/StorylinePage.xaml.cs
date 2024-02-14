using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для Storyline.xaml
    /// </summary>
    public partial class StoryLinePage : Page
    {
        private App _app;

        public StoryLinePage(MainWindow _mainWindow)
        {
            InitializeComponent();

            _app = (App)Application.Current;
            _app.ProcessLinesInBackground(this);
            
        }
        public async void StartProcessingLines(List<string> storyLinesFromApp, int delayInSeconds)
        {
            await Task.Run(async () =>
            {
                foreach (var line in storyLinesFromApp)
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        this.StoryListBox.Items.Add(line);
                    }, DispatcherPriority.Background);
                    await Task.Delay(delayInSeconds);
                }
            });
        }
    }
}
