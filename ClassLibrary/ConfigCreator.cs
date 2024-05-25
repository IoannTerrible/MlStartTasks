using System.Xml;

namespace ClassLibrary
{
    public class ConfigCreator
    {
        #region Public Methods
        public static void CreateDefaultConfigFile(string path)
        {
            try
            {
                XmlDocument xmlDoc = new();
                XmlElement root = xmlDoc.CreateElement("Config");
                xmlDoc.AppendChild(root);

                AddNetwork(xmlDoc, root);
                AddDatabase(xmlDoc, root);
                AddCheckRealTimeProcessVideo(xmlDoc, root);

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

        private static void AddNetwork(XmlDocument xmlDoc, XmlElement root)
        {
            XmlElement networkElement = xmlDoc.CreateElement("Network");
            root.AppendChild(networkElement);

            Dictionary<string, string> networkElementsDictionary = new()
            {
                { "Port", "8000" },
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

        private static void AddCheckRealTimeProcessVideo(XmlDocument xmlDoc, XmlElement root)
        {
            XmlElement videoElement = xmlDoc.CreateElement("VideoProcessing");
            root.AppendChild(videoElement);

            XmlElement connectionStringElement = xmlDoc.CreateElement("ProcessInRealTime");
            connectionStringElement.InnerText = "false";
            videoElement.AppendChild(connectionStringElement);

            XmlElement ClipLengthElement = xmlDoc.CreateElement("ClipLength");
            ClipLengthElement.InnerText = "6";
            videoElement.AppendChild(ClipLengthElement);
        }

        #endregion
    }
}
