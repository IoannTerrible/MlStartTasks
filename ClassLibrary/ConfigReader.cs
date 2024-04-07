using Serilog.Events;
using System.Xml;

namespace ClassLibrary
{
    public static class ConfigReader
    {
        public static Dictionary<string, string> ReadConfigFromFile(string path)
        {
            try
            {
                XmlDocument xmlDoc = new();
                xmlDoc.Load(path);

                Dictionary<string, string> configDictionary = new()
                {
                    { "Port", ReadNodeValue(xmlDoc, "//Config/Network/Port") },
                    { "Ip", ReadNodeValue(xmlDoc, "//Config/Network/Ip") },
                    { "ConnectionString", ReadNodeValue(xmlDoc, "//Config/Database/ConnectionString") }
                };

                return configDictionary;
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(LogEventLevel.Error, ex, "Error while read configfile");
                return new Dictionary<string, string>();
            }
        }
        public static void UpdateConfigValues(string path, Dictionary<string, string> updatedValues)
        {
            try
            {
                XmlDocument xmlDoc = new();
                xmlDoc.Load(path);

                UpdateNodeValues(xmlDoc, updatedValues);

                xmlDoc.Save(path);
                Logger.LogByTemplate(LogEventLevel.Information, note: "Config file updated successfully.");
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(LogEventLevel.Error, ex, $"An error occurred while updating the config file: {ex.Message}");
            }
        }

        public static void UpdateNodeValues(XmlDocument xmlDoc, Dictionary<string, string> updatedValues)
        {

            foreach (var kvp in updatedValues)
            {
                string xpath = GetXPathForNode(kvp.Key);
                XmlNode? node = xmlDoc.SelectSingleNode(xpath);
                if (node != null)
                {
                    node.InnerText = kvp.Value;
                    Logger.LogByTemplate(Serilog.Events.LogEventLevel.Debug, note: $"Updated config value for {kvp.Key}");
                }
                else
                {
                    throw new Exception($"Node with XPath '{xpath}' not found.");
                }
            }
        }

        private static string GetXPathForNode(string nodeName)
        {
            return $"//*[local-name()='{nodeName}']";
        }

        private static string ReadNodeValue(XmlDocument xmlDoc, string xpath)
        {
            XmlNode? node = xmlDoc.SelectSingleNode(xpath);
            if (node != null)
            {
                Logger.LogByTemplate(LogEventLevel.Debug, note: $"Read from congif {node.InnerText}");
                return node.InnerText;
            }
            throw new Exception($"Node with XPath '{xpath}' not found.");
        }
    }
}
