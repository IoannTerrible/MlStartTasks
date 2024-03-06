using System.Windows;

namespace SocketClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly Connector _socketClient;
        public bool isConnected;

        public MainWindow()
        {
            InitializeComponent();
            _socketClient = new Connector("localhost", 11000);
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
            MainFrame.Navigate(new StoryPage(this));
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