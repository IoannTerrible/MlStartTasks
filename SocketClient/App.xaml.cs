using System.Configuration;
using System.Data;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Windows;

namespace SocketClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        public static void Main(string[] args)
        {
            SocketClient.App app = new SocketClient.App();
            
            app.InitializeComponent();
            app.Run();
        }
        
    }

}
