using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SocketClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Connector _socketClient;

        public MainWindow()
        {
            InitializeComponent();
            _socketClient = new Connector("localhost", 11000);
            try
            {
                SendMessageAndReceive("MessageForYou");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private async void SendMessageAndReceive(string message)
        {
            await _socketClient.SendMessage(message);
            string response = await _socketClient.ReceiveMessage();
            MessageBox.Show(response);

            if (!message.Contains("<TheEnd>"))
                SendMessageAndReceive(message);
        }
        private void RegClick(object sender, RoutedEventArgs e)
        {
            SendMessageAndReceive("fff");
        }

        private void LogClick(object sender, RoutedEventArgs e)
        {

        }

        private void StoryClick(object sender, RoutedEventArgs e)
        {

        }

        private void DisconnectClick(object sender, RoutedEventArgs e)
        {

        }

        private void GetIpButton_click(object sender, RoutedEventArgs e)
        {

        }
    }
}