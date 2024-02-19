using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Hoster
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var host = new ServiceHost(typeof(ServerLibrary.LoreServise)))
            {
                host.Open();
                Console.WriteLine("StartHost");
                Console.ReadLine();
            }
        }
    }
}
