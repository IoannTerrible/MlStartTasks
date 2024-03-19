using ClassLibrary;
using Serilog.Events;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Input;

namespace SocketClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Connector _socketClient;
        public bool isConnected = false;
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
            switch (message)
            {
                case string msg when msg.Contains("LOR"):
                    Logger.LogByTemplate(LogEventLevel.Information, note: "Message contains LOR, start receiving");
                    await _socketClient.ReceiveLoreMessages();
                    Logger.LogByTemplate(LogEventLevel.Information, note: "Receive Complete");
                    break;
                case string msg when msg.Contains("DIS"):
                    Logger.LogByTemplate(LogEventLevel.Information, note: "Message contains DIS, Don't receiving");
                    break;
                default:
                    Logger.LogByTemplate(LogEventLevel.Information, note: "Message doesn't contain LOR, start receiving");
                    response = await _socketClient.ReceiveMessage();
                    Logger.LogByTemplate(LogEventLevel.Information, note: "Base receive complete");
                    Logger.LogByTemplate(LogEventLevel.Information, note: $"Response from server: {response}");
                    if (response != null)
                    {
                        if (response == "You have successfully logged in")
                        {
                            isLogin = true;
                            UserNameTextBox.Text = LogInPage.login;
                        }
                        MessageBox.Show(response);
                    }
                    break;
            }

            Logger.LogByTemplate(LogEventLevel.Debug, note: $"Message sent and response received: {message}"); ;
        }

        private void RegistrationClick(object sender, RoutedEventArgs e)
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

        private void LoginClick(object sender, RoutedEventArgs e)
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
        public void SetButtonToGo(bool goState)
        {
            if(activeStoryPage != null)
            {
                activeStoryPage.MakeGoToStop(goState);
            }
            else
            {
                Logger.LogByTemplate(LogEventLevel.Error, note: "ActiveStoryPage is null");
            }
        }
        public void SetStopRecivingLines()
        {
            if(activeStoryPage != null)
            {
                activeStoryPage.receivingLines = false;
            }
            else
            {
                Logger.LogByTemplate(LogEventLevel.Error,note:"ActiveStoryPage is null");
            }
        }

        private async void DisconnectClick(object sender, RoutedEventArgs e)
        {
            try
            {
                await SendMessageAndReceive("DIS");
                isConnected = false;
                Logger.LogByTemplate(LogEventLevel.Information, note: "Disconnected from server.");
                MessageBox.Show("You was disconnected");
                UserStatus.Text = "Disconnected";
            }
            catch(Exception ex)
            {
                Logger.LogByTemplate(LogEventLevel.Error, ex, note: "Error while trying to disconnect"); 
            }
            finally
            {
                _socketClient.Disconnect();
            }
        }

        private async void ConnectionClick(object sender, RoutedEventArgs e)
        {
            if (isConnected)
            {
                MessageBox.Show("Already connected to a server.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            try
            {
                if (!int.TryParse(UserPortBox.Text, out int tempIntPort))
                {
                    MessageBox.Show("Invalid port number. Please enter a valid integer value.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var result = MessageBox.Show($"Are you sure? You try to connect to " +
                        $"port: {tempIntPort} " +
                        $"ip:{UserIpBox.Text}",
                        "Confirm Connection", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _socketClient = new Connector(UserIpBox.Text, tempIntPort, this);
                    await _socketClient.Connect();
                    UserIpBox.Text = _socketClient._host;
                    await SendMessageAndReceive("CON");
                    isConnected = true;
                    UserStatus.Text = ("Connected");
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(LogEventLevel.Error, ex, "Error while trying to connect");
                MessageBox.Show("An error occurred while trying to connect. Please try again later.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Window_Closed(object sender, EventArgs e)
        {
            await SendMessageAndReceive("DIS");
            _socketClient.sender.Shutdown(SocketShutdown.Both);
            _socketClient.sender.Close();
        }
    }
}