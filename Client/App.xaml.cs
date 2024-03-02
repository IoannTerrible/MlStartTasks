using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Serilog.Events;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using static Serilog.Events.LogEventLevel;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string[] realContent { get; set; }
        [STAThread]
        public static void Main(string[] args)
        {
            ClientLogger.CreateLogDirectory(
                Debug,
                Information,
                Warning,
                Error
            );
            Client.App app = new Client.App();
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = System.IO.Path.Combine(currentDirectory, "config.xml");

            ClientLogger.LogByTemplate(Debug, note: "Checking and configuring file ");
            ClientLogger.LogByTemplate(Information, note: $"Config file path: {filePath}");

            if (!File.Exists(filePath))
            {
                ClientLogger.LogByTemplate(Debug, note: "Config file not found, creating with default content ");
                ConfigCreator.CreateDefaultConfigFile(filePath);
            }

            realContent = ConfigReader.ReadConfigFromFile(filePath);

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
