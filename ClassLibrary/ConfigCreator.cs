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
                { "DelayInSeconds", "1" }
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
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement("Config");
            xmlDoc.AppendChild(root);

            var numbersDictionary = new Dictionary<string, string>
        {
            { "Number1", "7" },
            { "Number2", "5" },
            {"DelayInSeconds", "2" }
        };

            foreach (var pair in numbersDictionary)
            {
                XmlElement numberElement = xmlDoc.CreateElement(pair.Key);
                numberElement.InnerText = pair.Value;
                root.AppendChild(numberElement);
            }

            // Adding database connection string
            XmlElement connectionStringElement = xmlDoc.CreateElement("ConnectionString");
            connectionStringElement.InnerText = "server=(localdb)\\MSSqlLocalDb;Trusted_Connection=Yes;DataBase=MLstartDataBase;";
            root.AppendChild(connectionStringElement);

            xmlDoc.Save(path);
        }
    }
}
