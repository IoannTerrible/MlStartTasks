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
        public readonly Connector _socketClient;

        public MainWindow()
        {
            InitializeComponent();
            _socketClient = new Connector("localhost", 11000);
            //try
            //{
            //    _socketClient.Connect();
            //}
            //catch (Exception ex)
            //{
            //    //MessageBox.Show(ex.ToString() + "Connection Failed");
            //}
        }
        public async void SendMessageAndReceive(string message)
        {
            await _socketClient.SendMessage(message);
            string response = await _socketClient.ReceiveMessage();
            MessageBox.Show(response);

        }
        private void RegClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new RegInPage(this));
        }

        private void LogClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new LogInPage(this));
        }

        private void StoryClick(object sender, RoutedEventArgs e)
        {

        }

        private void DisconnectClick(object sender, RoutedEventArgs e)
        {
            _socketClient.Disconnect();
        }

        private void GetIpButton_click(object sender, RoutedEventArgs e)
        {

        }
    }
}