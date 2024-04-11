using System.IO;
using System.Windows;
using ClassLibrary;

using static Serilog.Events.LogEventLevel;


namespace Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Dictionary<string, string> ContentFromConfig { get; set; }
        public static string PathToConfig { get; set; }
        [STAThread]
        public static void Main(string[] args)
        {
            Logger.CreateLogDirectory(
                Debug,
                Information,
                Warning,
                Error
            );
            Client.App app = new Client.App();
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            PathToConfig = System.IO.Path.Combine(currentDirectory, "config.xml");

            Logger.LogByTemplate(Debug, note: "Checking and configuring file for client");
            Logger.LogByTemplate(Information, note: $"Config file path: {PathToConfig}");

            if (!File.Exists(PathToConfig))
            {
                Logger.LogByTemplate(Debug, note: "Config file not found, creating with default content ");
                ConfigCreator.CreateDefaultConfigFile(PathToConfig);
            }

            ContentFromConfig = ConfigReader.ReadConfigFromFile(PathToConfig);

            app.InitializeComponent();
            app.Run();
        }
    }

}
