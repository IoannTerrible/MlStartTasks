using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ClassLibrary
{
    public class ConfigCreator
    {
        public static void CreateDefaultConfigFileForClient(string path)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement("Config");
            xmlDoc.AppendChild(root);

            var numbersDictionary = new Dictionary<string, string>
            {
                { "DelayInSeconds", "2" }
            };

            foreach (var pair in numbersDictionary)
            {
                XmlElement numberElement = xmlDoc.CreateElement(pair.Key);
                numberElement.InnerText = pair.Value;
                root.AppendChild(numberElement);
            }

            xmlDoc.Save(path);
        }
        public static void CreateDefaultConfigFileForServer(string path)
        {
            try
            {
                var xmlDoc = new XmlDocument();
                var root = xmlDoc.CreateElement("Config");
                xmlDoc.AppendChild(root);

                var numbersNode = xmlDoc.CreateElement("Numbers");
                root.AppendChild(numbersNode);

                var numbersDictionary = new Dictionary<string, string>
                {
                    { "Number1", "7" },
                    { "Number2", "5" }
                };

                foreach (var pair in numbersDictionary)
                {
                    var numberElement = xmlDoc.CreateElement(pair.Key);
                    numberElement.InnerText = pair.Value;
                    numbersNode.AppendChild(numberElement);
                }

                var networkElement = xmlDoc.CreateElement("Network");
                root.AppendChild(networkElement);

                var networkElementsDictionary = new Dictionary<string, string>
                {
                    { "Port", "11000" },
                    { "Ip", "localhost" }
                };

                foreach (var pair in networkElementsDictionary)
                {
                    var element = xmlDoc.CreateElement(pair.Key);
                    element.InnerText = pair.Value;
                    networkElement.AppendChild(element);
                }

                var databaseElement = xmlDoc.CreateElement("Database");
                root.AppendChild(databaseElement);

                var connectionStringElement = xmlDoc.CreateElement("ConnectionString");
                connectionStringElement.InnerText = @"server=(localdb)\MSSqlLocalDb; Trusted_Connection = Yes; DataBase = MLstartDataBase";
                databaseElement.AppendChild(connectionStringElement);

                var otherElementsDictionary = new Dictionary<string, string>
                {
                    { "DelayInSeconds", "2" }
                };

                foreach (var pair in otherElementsDictionary)
                {
                    var element = xmlDoc.CreateElement(pair.Key);
                    element.InnerText = pair.Value;
                    root.AppendChild(element);
                }

                xmlDoc.Save(path);
                Logger.LogByTemplate(Serilog.Events.LogEventLevel.Information, note: "Default configuration file created successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(Serilog.Events.LogEventLevel.Error, ex, $"An error occurred while creating the config file: {ex.Message}");
            }
        }

    }
}


