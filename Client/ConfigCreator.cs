using System.Collections.Generic;
using System.Xml;

namespace Client
{
    public class ConfigCreator
    {
        public static void CreateDefaultConfigFile(string path)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement("Config");
            xmlDoc.AppendChild(root);

            var numbersDictionary = new Dictionary<string, string>
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

            xmlDoc.Save(path);
        }
    }
}
