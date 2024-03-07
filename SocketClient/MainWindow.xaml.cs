using Serilog.Events;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
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
        public bool isLogin;
        public string response;
        public StoryPage activeStoryPage;
        private App _app;
        public MainWindow()
        {
            InitializeComponent();
            _app = (App)Application.Current;
            _socketClient = new Connector("localhost", 11000, this);
        }
        public async Task SendMessageAndReceive(string message)
        {
            await _socketClient.SendMessage(message);
            if (message.Contains("LOR") || message.Contains("CNF"))
            {
                await _socketClient.ReceiveLoreMessages();
            }
            else
            {
                response = await _socketClient.ReceiveMessage();
                if (response != null)
                {
                    if (response == "You have successfully logged in")
                    {
                        isLogin = true;
                    }
                    MessageBox.Show(response);
                }
            }

            ClientLogger.LogByTemplate(LogEventLevel.Debug, note: $"Message sent and response received: {message}"); ;
        }

        private void RegClick(object sender, RoutedEventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("Need to Connect");
            }
            else
            {
                ClientLogger.LogByTemplate(LogEventLevel.Information, note: "Registration page opened.");
                MainFrame.Navigate(new RegInPage(this));
            }
        }

        private void LogClick(object sender, RoutedEventArgs e)
        {
            ClientLogger.LogByTemplate(LogEventLevel.Information, note: "Login page opened.");
            MainFrame.Navigate(new LogInPage(this));
        }

        private void StoryClick(object sender, RoutedEventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("Need Connect to Server");
                ClientLogger.LogByTemplate(LogEventLevel.Warning, note: "Attempt to access story page without connection.");
            }
            if (!isLogin)
            {
                MessageBox.Show("Need Login to Server");
                ClientLogger.LogByTemplate(LogEventLevel.Warning, note: "Attempt to access story page without connection.");
            }
            else
            {
                try
                {
                    string configString = "CNF" + " " + string.Join(" ", App.contentFromConfig);
                    SendMessageAndReceive(configString);
                    response = string.Empty;
                    activeStoryPage = new StoryPage(this);
                    MainFrame.Navigate(activeStoryPage);
                    ClientLogger.LogByTemplate(LogEventLevel.Information, note: "Story page opened.");
                }
                catch (Exception ex)
                {
                    ClientLogger.LogByTemplate(LogEventLevel.Error, ex, "Error occurred while processing StoryClick event.");
                }
            }
        }

        private void DisconnectClick(object sender, RoutedEventArgs e)
        {
            _socketClient.Disconnect();
            isConnected = false;
            ClientLogger.LogByTemplate(LogEventLevel.Information, note: "Disconnected from server.");
        }

        private void GetIpButton_click(object sender, RoutedEventArgs e)
        {
            _socketClient.Connect();
            UserIpBox.Text = _socketClient._host;
            SendMessageAndReceive("CON");
            isConnected = true;
            ClientLogger.LogByTemplate(LogEventLevel.Information, note: "Connected to server.");
        }
    }
}