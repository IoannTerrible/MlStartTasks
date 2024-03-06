using System.IO;
using System.Windows;

using static Serilog.Events.LogEventLevel;


namespace SocketClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string[] contentFromConfig { get; set; }

        [STAThread]
        public static void Main(string[] args)
        {
            ClientLogger.CreateLogDirectory(
                Debug,
                Information,
                Warning,
                Error
            );
            SocketClient.App app = new SocketClient.App();
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = System.IO.Path.Combine(currentDirectory, "config.xml");

            ClientLogger.LogByTemplate(Debug, note: "Checking and configuring file ");
            ClientLogger.LogByTemplate(Information, note: $"Config file path: {filePath}");

            if (!File.Exists(filePath))
            {
                ClientLogger.LogByTemplate(Debug, note: "Config file not found, creating with default content ");
                ConfigCreator.CreateDefaultConfigFile(filePath);
            }

            contentFromConfig = ConfigReader.ReadConfigFromFile(filePath);

            app.InitializeComponent();
            app.Run();
        }
        public async Task ProcessLinesInBackground(StoryPage storyPage)
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (storyPage != null)
                    {
                        storyPage.StartProcessingLines(storyPage.lines, storyPage.delays);
                    }
                });
            });
        }
    }

}
