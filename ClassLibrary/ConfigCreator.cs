using System.Xml;

namespace ClassLibrary
{
    public class ConfigCreator
    {
        #region Public Methods

        public static void CreateDefaultConfigFileForClient(string path)
        {
            XmlDocument xmlDoc = new();
            XmlElement root = xmlDoc.CreateElement("Config");
            xmlDoc.AppendChild(root);

            AddDatabase(xmlDoc, root);

            Dictionary<string, string> numbersDictionary = new()
            {
                { "DelayInMilliseconds", "2000" }
            };

            foreach (var pair in numbersDictionary)
            {
                XmlElement numberElement = xmlDoc.CreateElement(pair.Key);
                numberElement.InnerText = pair.Value;
                root.AppendChild(numberElement);
            }

            xmlDoc.Save(path);
        }

        public static void CreateDefaultConfigFile(string path)
        {
            try
            {
                XmlDocument xmlDoc = new();
                XmlElement root = xmlDoc.CreateElement("Config");
                xmlDoc.AppendChild(root);

                AddNumbers(xmlDoc, root);
                AddNetwork(xmlDoc, root);
                AddDatabase(xmlDoc, root);
                AddOtherElements(xmlDoc, root);

                xmlDoc.Save(path);
                Logger.LogByTemplate(Serilog.Events.LogEventLevel.Information, note: "Default configuration file created successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(Serilog.Events.LogEventLevel.Error, ex, $"An error occurred while creating the config file: {ex.Message}");
            }
        }

        #endregion

        #region Private Methods

        private static void AddNumbers(XmlDocument xmlDoc, XmlElement root)
        {
            XmlElement numbersNode = xmlDoc.CreateElement("Numbers");
            root.AppendChild(numbersNode);

            Dictionary<string, string> numbersDictionary = new()
            {
                { "Number1", "7" },
                { "Number2", "5" }
            };

            foreach (var pair in numbersDictionary)
            {
                XmlElement numberElement = xmlDoc.CreateElement(pair.Key);
                numberElement.InnerText = pair.Value;
                numbersNode.AppendChild(numberElement);
            }
        }

        private static void AddNetwork(XmlDocument xmlDoc, XmlElement root)
        {
            XmlElement networkElement = xmlDoc.CreateElement("Network");
            root.AppendChild(networkElement);

            Dictionary<string, string> networkElementsDictionary = new()
            {
                { "Port", "11000" },
                { "Ip", "localhost" }
            };

            foreach (var pair in networkElementsDictionary)
            {
                XmlElement element = xmlDoc.CreateElement(pair.Key);
                element.InnerText = pair.Value;
                networkElement.AppendChild(element);
            }
        }

        private static void AddDatabase(XmlDocument xmlDoc, XmlElement root)
        {
            XmlElement databaseElement = xmlDoc.CreateElement("Database");
            root.AppendChild(databaseElement);

            XmlElement connectionStringElement = xmlDoc.CreateElement("ConnectionString");
            connectionStringElement.InnerText = @"server=(localdb)\MSSqlLocalDb; Trusted_Connection = Yes; DataBase = MLstartDataBase";
            databaseElement.AppendChild(connectionStringElement);
        }

        private static void AddOtherElements(XmlDocument xmlDoc, XmlElement root)
        {
            Dictionary<string, string> otherElementsDictionary = new()
            {
                { "DelayInMilliseconds", "2000" }
            };

            foreach (KeyValuePair<string, string> pair in otherElementsDictionary)
            {
                XmlElement element = xmlDoc.CreateElement(pair.Key);
                element.InnerText = pair.Value;
                root.AppendChild(element);
            }
        }

        #endregion
    }
}
