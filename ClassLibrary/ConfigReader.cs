using Serilog.Events;
using System.Xml;

namespace ClassLibrary
{
    public static class ConfigReader
    {
        public static string[] ReadConfigFromFile(string path)
        {
            try
            {
                XmlDocument xmlDoc = new();
                xmlDoc.Load(path);

                List<string> values = new List<string>
                {
                    ReadNodeValue(xmlDoc, "//Config/Numbers/Number1"),
                    ReadNodeValue(xmlDoc, "//Config/Numbers/Number2"),
                    ReadNodeValue(xmlDoc, "//Config/DelayInMilliseconds"),
                    ReadNodeValue(xmlDoc, "//Config/Network/Port"),
                    ReadNodeValue(xmlDoc, "//Config/Network/Ip"),
                    ReadNodeValue(xmlDoc, "//Config/Database/ConnectionString")
                };

                return values.ToArray();
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(LogEventLevel.Error,ex,"Error while read configfile");
                return new string[0]; 
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
