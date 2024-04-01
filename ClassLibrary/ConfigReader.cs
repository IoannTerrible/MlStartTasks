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
                    { "Number1", ReadNodeValue(xmlDoc, "//Config/Numbers/Number1") },
                    { "Number2", ReadNodeValue(xmlDoc, "//Config/Numbers/Number2") },
                    { "DelayInMilliseconds", ReadNodeValue(xmlDoc, "//Config/DelayInMilliseconds") },
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
