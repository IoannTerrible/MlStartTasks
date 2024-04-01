using ClassLibrary;
using Microsoft.Win32;
using Serilog.Events;
using System.IO;
using System.Net.Http;
using System.Windows;
using static System.Net.WebRequestMethods;

namespace SocketClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public Connector _socketClient;
        //public bool isLogin;
        //public string response;
        //public StoryPage activeStoryPage;
        public ImagePage activyImagePage;
        public static HttpClient client = new();
        private App _app;
        private static ApiClient _apiClient;

        public MainWindow()
        {
            InitializeComponent();
            _app = (App)Application.Current;
            _apiClient = new ApiClient(this);
        }
        #region OldSocketClient
        //public async Task SendMessageAndReceive(string message)
        //{
        //    Logger.LogByTemplate(LogEventLevel.Information, note: "Start Send message");
        //    await _socketClient.SendMessage(message);
        //    Logger.LogByTemplate(LogEventLevel.Information, note: "Message was sent");
        //    switch (message)
        //    {
        //        case string msg when msg.Contains("LOR"):
        //            Logger.LogByTemplate(LogEventLevel.Information, note: "Message contains LOR, start receiving");
        //            await _socketClient.ReceiveLoreMessages();
        //            Logger.LogByTemplate(LogEventLevel.Information, note: "Receive Complete");
        //            break;
        //        case string msg when msg.Contains("DIS"):
        //            Logger.LogByTemplate(LogEventLevel.Information, note: "Message contains DIS, Don't receiving");
        //            break;
        //        default:
        //            Logger.LogByTemplate(LogEventLevel.Information, note: "Message doesn't contain LOR, start receiving");
        //            response = await _socketClient.ReceiveMessage();
        //            Logger.LogByTemplate(LogEventLevel.Information, note: "Base receive complete");
        //            Logger.LogByTemplate(LogEventLevel.Information, note: $"Response from server: {response}");
        //            if (response != null)
        //            {
        //                if (response == "You have successfully logged in")
        //                {
        //                    isLogin = true;
        //                    UserNameTextBox.Text = LogInPage.login;
        //                }
        //                MessageBox.Show(response);
        //            }
        //            break;
        //    }

        //    Logger.LogByTemplate(LogEventLevel.Debug, note: $"Message sent and response received: {message}"); ;
        //}
        private void RegistrationClick(object sender, RoutedEventArgs e)
        {
                //Logger.LogByTemplate(LogEventLevel.Information, note: "Registration page opened.");
                //MainFrame.Navigate(new RegInPage(this));
        }
        private void LoginClick(object sender, RoutedEventArgs e)
        {
            //Logger.LogByTemplate(LogEventLevel.Information, note: "Login page opened.");
            //MainFrame.Navigate(new LogInPage(this));
        }
        //private void StoryClick(object sender, RoutedEventArgs e)
        //{
        //    if (!isLogin)
        //    {
        //        MessageBox.Show("Need Login to Server");
        //        Logger.LogByTemplate(LogEventLevel.Warning, note: "Attempt to access story page without connection.");
        //    }
        //    else
        //    {
        //        try
        //        {

        //            activeStoryPage = new StoryPage(this);
        //            MainFrame.Navigate(activeStoryPage);
        //            Logger.LogByTemplate(LogEventLevel.Information, note: "Story page opened.");
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger.LogByTemplate(LogEventLevel.Error, ex, "Error occurred while processing StoryClick event.");
        //        }
        //    }
        //}
        //public void SetButtonToGo(bool goState)
        //{
        //    if(activeStoryPage != null)
        //    {
        //        activeStoryPage.MakeGoToStop(goState);
        //    }
        //    else
        //    {
        //        Logger.LogByTemplate(LogEventLevel.Error, note: "ActiveStoryPage is null");
        //    }
        //}
        //public void SetStopRecivingLines()
        //{
        //    if(activeStoryPage != null)
        //    {
        //        activeStoryPage.receivingLines = false;
        //    }
        //    else
        //    {
        //        Logger.LogByTemplate(LogEventLevel.Error,note:"ActiveStoryPage is null");
        //    }
        //}
        #endregion
        private async void ImagePageClick(object sender, RoutedEventArgs e)
        {
            activyImagePage = new ImagePage(this);
            MainFrame.Navigate(activyImagePage);
        }
        private async void DisconnectClick(object sender, RoutedEventArgs e)
        {
            ConnectionWindow.ConnectionUri = null;
            UserStatus.Text = "YouAreDisconnect";
        }

        private async void ConnectionClick(object sender, RoutedEventArgs e)
        {
            ConnectionWindow.ShowConnectionDialog();
            UserStatus.Text = ConnectionWindow.ConnectionUri.ToString();
        }
        public static void ReciveResponce(string responce)
        {
            MessageBox.Show(responce);
        }
        public static async Task PerfomHealthChekAsync()
        {
            try
            
            {
                if (ConnectionWindow.ConnectionUri == null)
                {
                    MessageBox.Show("Need to press the Connect button.");
                    return;
                }
                if (await _apiClient.CheckHealthAsync($"{ConnectionWindow.ConnectionUri}health"))
                {
                    MessageBox.Show("All good! The health check succeeded.");
                    Logger.LogByTemplate(LogEventLevel.Information, note: "Health check succeeded.");
                }
                else
                {
                    MessageBox.Show("Health check returned false");
                    Logger.LogByTemplate(LogEventLevel.Warning, note: "Health check returned false.");
                }
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogByTemplate(LogEventLevel.Error, httpEx, note: "HTTP request error while performing health check.");
                MessageBox.Show($"HTTP request error: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(LogEventLevel.Error, ex, note: "Error while performing health check.");
                MessageBox.Show($"An unexpected error occurred: {ex.Message}");
            }
        }

        public static async void SendImage()
        {
            try
            {
                if(ConnectionWindow.ConnectionUri == null)
                {
                    MessageBox.Show("Need press Connect button");
                    return;
                }
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = false;
                openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == true)
                {
                    string filename = openFileDialog.FileName;
                    string shortFileName = Logger.GetLastFile(filename);
                    if (!System.IO.File.Exists(filename))
                    {
                        Logger.LogByTemplate(LogEventLevel.Error, note: "Selected file does not exist: " + shortFileName);
                        return;
                    }

                    Logger.LogByTemplate(LogEventLevel.Information, note: "Sending image: " + shortFileName);
                    await _apiClient.SendImageAndReceiveJSONAsync(filename, ConnectionWindow.ConnectionUri);
                    Logger.LogByTemplate(LogEventLevel.Information, note: "Image sent successfully: " + shortFileName);
                }
                else
                {
                    Logger.LogByTemplate(LogEventLevel.Warning, note: "No file selected.");
                }
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogByTemplate(LogEventLevel.Error, httpEx, note: "HTTP request error while performing health check.");
                MessageBox.Show($"HTTP request error: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(LogEventLevel.Error, ex, note: "Error while performing health check.");
                MessageBox.Show($"An unexpected error occurred: {ex.Message}");
            }
        }

    }
}