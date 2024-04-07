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
        public static ApiClient apiClient;

        public static HttpClient client = new();

        private App _app;
        

        public MainWindow()
        {
            InitializeComponent();
            _app = (App)Application.Current;
            apiClient = new ApiClient(this);
        }

        private async void FastConnectClick(object sender, RoutedEventArgs e)
        {

            ConnectionWindow.ConnectionUri = $"http://{App.ContentFromConfig["Ip"]}:{App.ContentFromConfig["Port"]}/";
            Logger.LogByTemplate(LogEventLevel.Information, note: "Used fast connection to localhost:8000.");
            UserStatus.Text = ConnectionWindow.ConnectionUri.ToString();
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
            activyVideoPage = new VideoPage(this);
            MainFrame.Navigate(activyVideoPage);
        }
        private async void ConfigClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ConfigPage(this));
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

                loginButton.Visibility = Visibility.Collapsed;
                registrationButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                fastConnectButton.Visibility = Visibility.Collapsed;
                disconButton.Visibility = Visibility.Collapsed;
                imagePageButton.Visibility = Visibility.Collapsed;
                configButton.Visibility = Visibility.Collapsed;
                connectButton.Visibility = Visibility.Collapsed;

                loginButton.Visibility = Visibility.Visible;
                registrationButton.Visibility = Visibility.Visible;
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
                if (await apiClient.CheckHealthAsync($"{ConnectionWindow.ConnectionUri}health"))
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
                    await apiClient.SendImageAndReceiveJSONAsync(openedFile, ConnectionWindow.ConnectionUri);
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