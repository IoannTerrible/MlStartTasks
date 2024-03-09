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
        public static string[] contentFromConfig { get; set; }

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

            Logger.LogByTemplate(Debug, note: "Checking and configuring file ");
            Logger.LogByTemplate(Information, note: $"Config file path: {filePath}");

            if (!File.Exists(filePath))
            {
                ClassLibrary.Logger.LogByTemplate(Debug, note: "Config file not found, creating with default content ");
                ConfigCreator.CreateDefaultConfigFile(filePath);
            }

            contentFromConfig = ConfigReader.ReadConfigFromFile(filePath);

            app.InitializeComponent();
            app.Run();
        }
    }

}
