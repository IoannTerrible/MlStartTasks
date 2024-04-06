using ClassLibrary;
using Client;
using Serilog.Events;
using System.Net.Http;
using System.Windows;

namespace SocketClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string connectionString = App.ContentFromConfig["ConnectionString"];
        public bool areWeLogin = false;

        public ImagePage activyImagePage;
        public VideoPage activyVideoPage;

        public static HttpClient client = new();

        private App _app;
        private static ApiClient _apiClient;

        public MainWindow()
        {
            InitializeComponent();
            _app = (App)Application.Current;
            _apiClient = new ApiClient(this);
        }

        private async void FastConnectClick(object sender, RoutedEventArgs e)

        {
            ConnectionWindow.ConnectionUri = @"http://localhost:8000/";
            Logger.LogByTemplate(LogEventLevel.Information, note: "Used fast connection to localhost:8000.");
        }
        private async void ConnectionClick(object sender, RoutedEventArgs e)
        {
            ConnectionWindow.ShowConnectionDialog();
            if (ConnectionWindow.ConnectionUri != null)
            {
                UserStatus.Text = ConnectionWindow.ConnectionUri.ToString();
            }
        }
        private async void DisconnectClick(object sender, RoutedEventArgs e)
        {
            ConnectionWindow.ConnectionUri = null;
            UserStatus.Text = "YouAreDisconnect";
            Logger.LogByTemplate(LogEventLevel.Information, note: "Disconnected from the server.");
        }
        private async void ImagePageClick(object sender, RoutedEventArgs e)
        {
            activyVideoPage = new VideoPage();
            MainFrame.Navigate(activyVideoPage);
        }
        private async void ConfigClick(object sender, RoutedEventArgs e)
        {
            // open configuration
            ConnectionWindow.ShowConnectionDialog();
            if (ConnectionWindow.ConnectionUri != null)
            {
                UserStatus.Text = ConnectionWindow.ConnectionUri.ToString();
            }
        }
        private void RegistrPageClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new RegInPage(this));
        }

        private void LoginPageClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new LogInPage(this));
        }
        public void UpdateButtonVisibility(bool areWeLoggedIn)
        {
            if (areWeLoggedIn)
            {
                fastConnectButton.Visibility = Visibility.Visible;
                disconButton.Visibility = Visibility.Visible;
                imagePageButton.Visibility = Visibility.Visible;
                configButton.Visibility = Visibility.Visible;
                connectButton.Visibility = Visibility.Visible;
            }
            else
            {
                fastConnectButton.Visibility = Visibility.Collapsed;
                disconButton.Visibility = Visibility.Collapsed;
                imagePageButton.Visibility = Visibility.Collapsed;
                configButton.Visibility = Visibility.Collapsed;
                connectButton.Visibility = Visibility.Collapsed;
            }
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

                string? openedFile = FileHandler.OpenFile("Image");

                if(openedFile != null)
                {
                    Uri uriFile = new(openedFile);
                    Logger.LogByTemplate(LogEventLevel.Information, note: "Sending image: " + openedFile);
                    await _apiClient.SendImageAndReceiveJSONAsync(openedFile, ConnectionWindow.ConnectionUri);
                    Logger.LogByTemplate(LogEventLevel.Information, note: "Image sent successfully: " + openedFile);
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