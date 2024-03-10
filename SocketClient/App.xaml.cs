using System.IO;
using System.Windows;
using ClassLibrary;

using static Serilog.Events.LogEventLevel;


namespace SocketClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string[] ContentFromConfig { get; set; }

        [STAThread]
        public static void Main(string[] args)
        {
            Logger.CreateLogDirectory(
                Debug,
                Information,
                Warning,
                Error
            );
            SocketClient.App app = new SocketClient.App();
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = System.IO.Path.Combine(currentDirectory, "config.xml");

            Logger.LogByTemplate(Debug, note: "Checking and configuring file for client");
            Logger.LogByTemplate(Information, note: $"Config file path: {filePath}");

            if (!File.Exists(filePath))
            {
                Logger.LogByTemplate(Debug, note: "Config file not found, creating with default content ");
                ConfigCreator.CreateDefaultConfigFileForClient(filePath);
            }

            ContentFromConfig = ConfigReader.ReadConfigFromFile(filePath);

            app.InitializeComponent();
            app.Run();
        }
    }

}
