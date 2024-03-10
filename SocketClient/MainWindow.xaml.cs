using Serilog.Events;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows;
using ClassLibrary;

namespace SocketClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Connector _socketClient;
        public bool isConnected;
        public bool isLogin;
        public string response;
        public StoryPage activeStoryPage;
        private App _app;
        public MainWindow()
        {
            InitializeComponent();
            _app = (App)Application.Current;
        }
        public async Task SendMessageAndReceive(string message)
        {
            Logger.LogByTemplate(LogEventLevel.Information, note: "Start Send message");
            await _socketClient.SendMessage(message);
            Logger.LogByTemplate(LogEventLevel.Information, note: "Message was sent");
            if (message.Contains("LOR"))
            {
                Logger.LogByTemplate(LogEventLevel.Information, note: "Message contains LOR, start reciving");
                await _socketClient.ReceiveLoreMessages();
                Logger.LogByTemplate(LogEventLevel.Information, note: "Recive Complete");
            }
            else
            {
                Logger.LogByTemplate(LogEventLevel.Information, note: "Message don't contains LOR start reciving");
                response = await _socketClient.ReceiveMessage();
                Logger.LogByTemplate(LogEventLevel.Information, note: "Base recive complete");
                Logger.LogByTemplate(LogEventLevel.Information, note: $"Response from server {response}");
                if (response != null)
                {                  
                    if (response == "You have successfully logged in")
                    {
                        isLogin = true;
                    }
                    MessageBox.Show(response);
                }
            }

            Logger.LogByTemplate(LogEventLevel.Debug, note: $"Message sent and response received: {message}"); ;
        }

        private void RegClick(object sender, RoutedEventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("Need to Connect");
            }
            else
            {
                Logger.LogByTemplate(LogEventLevel.Information, note: "Registration page opened.");
                MainFrame.Navigate(new RegInPage(this));
            }
        }

        private void LogClick(object sender, RoutedEventArgs e)
        {
            Logger.LogByTemplate(LogEventLevel.Information, note: "Login page opened.");
            MainFrame.Navigate(new LogInPage(this));
        }

        private void StoryClick(object sender, RoutedEventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("Need Connect to Server");
                Logger.LogByTemplate(LogEventLevel.Warning, note: "Attempt to access story page without connection.");
            }
            if (!isLogin)
            {
                MessageBox.Show("Need Login to Server");
                Logger.LogByTemplate(LogEventLevel.Warning, note: "Attempt to access story page without connection.");
            }
            else
            {
                try
                {

                    activeStoryPage = new StoryPage(this);
                    MainFrame.Navigate(activeStoryPage);
                    Logger.LogByTemplate(LogEventLevel.Information, note: "Story page opened.");
                }
                catch (Exception ex)
                {
                    Logger.LogByTemplate(LogEventLevel.Error, ex, "Error occurred while processing StoryClick event.");
                }
            }
        }

        private void DisconnectClick(object sender, RoutedEventArgs e)
        {
            _socketClient.Disconnect();
            isConnected = false;
            Logger.LogByTemplate(LogEventLevel.Information, note: "Disconnected from server.");
        }

        private async void GetIpButton_click(object sender, RoutedEventArgs e)
        {
            if (isConnected == false)
            {
                _socketClient = new Connector("localhost", 11000, this);
            }
            await _socketClient.Connect();

            UserIpBox.Text = _socketClient._host;
            await SendMessageAndReceive("CON");
            isConnected = true;

            //Logger.LogByTemplate(LogEventLevel.Information, note: "Connected to server.");
            //string configString = "CNF" + " " + string.Join(" ", App.ContentFromConfig);
            //SendMessageAndReceive(configString);
            //Logger.LogByTemplate(LogEventLevel.Information, note: "Send Config to server");


            //response = string.Empty;
        }
    }
}