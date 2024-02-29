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
            Client.App app = new Client.App();
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = System.IO.Path.Combine(currentDirectory, "config.xml");

            ClientLogger.LogByTemplate(Debug, note: "Checking and configuring file ");
            ClientLogger.LogByTemplate(Information, note: $"Config file path: {filePath}");

            if (!File.Exists(filePath))
            {
                ClientLogger.LogByTemplate(Debug, note: "Config file not found, creating with default content ");
                CreateDefaultConfigFile(filePath);
            }

            realContent = ReadConfigFromFile(filePath);


            void CreateDefaultConfigFile(string Path)
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlElement root = xmlDoc.CreateElement("Config");
                xmlDoc.AppendChild(root);
                Dictionary<string, string> numbersDictionary = new Dictionary<string, string>
                {
                { "Number1", "7" },
                { "Number2", "9" },
                { "Delay", "5" }
                };
                foreach (var pair in numbersDictionary)
                {
                    XmlElement numberElement = xmlDoc.CreateElement(pair.Key);
                    numberElement.InnerText = pair.Value;
                    root.AppendChild(numberElement);
                }

                xmlDoc.Save(Path);
            }

            string[] ReadConfigFromFile(string Path2)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(Path2);

                XmlNodeList elements = xmlDoc.SelectNodes("//text()");

                if (elements != null)
                {
                    List<string> values = new List<string>();

                    foreach (XmlNode element in elements)
                    {
                        if (element.ParentNode != null && element.ParentNode.NodeType == XmlNodeType.Element)
                        {
                            values.Add(element.InnerText);
                        }
                    }

                    return values.ToArray();
                }
                else
                {
                    ClientLogger.LogByTemplate(logEventLevel: LogEventLevel.Error, note: "Content node not found in the config file.");
                    throw new InvalidOperationException("Content node not found in the config file.");
                }
            }
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
