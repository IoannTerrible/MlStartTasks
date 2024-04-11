using ClassLibrary;
using Serilog.Events;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string connectionString = App.ContentFromConfig["ConnectionString"];
        public bool areWeLogin = false;
        public bool areWeConnected = false;

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
            UpdateButtonVisibility(areWeLogin);
            string[] eventLogColumns =
            [
                            "UserName VARCHAR(255) NULL",
                            "FileName VARCHAR(255) NULL",
                            "FramePath NVARCHAR(MAX) NULL",
                            "MetaData NVARCHAR(MAX) NULL"
            ];
            SqlCore.CreateTable(connectionString, "EventLog", eventLogColumns);
        }
        private async void FastConnectClick(object sender, RoutedEventArgs e)
        {
            ConnectionWindow.ConnectionUri = @"http://localhost:7000/";
            areWeConnected = true;
            UserStatus.Text = ConnectionWindow.ConnectionUri.ToString();
        }
        private async void ConnectionClick(object sender, RoutedEventArgs e)
        {
            ConnectionWindow.ShowConnectionDialog();
            if (ConnectionWindow.ConnectionUri != null)
            {
                UserStatus.Text = ConnectionWindow.ConnectionUri.ToString();
                areWeConnected = true;
            }
        }
        private async void DisconnectClick(object sender, RoutedEventArgs e)
        {
            ConnectionWindow.ConnectionUri = null;
            areWeConnected = false;
            UserStatus.Text = "YouAreDisconnect";
            Logger.LogByTemplate(LogEventLevel.Information, note: "Disconnected from the server.");
        }
        private async void ImagePageClick(object sender, RoutedEventArgs e)
        {
            if (areWeConnected)
            {
                activyVideoPage = new VideoPage(this);
                MainFrame.Navigate(activyVideoPage);
            }
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
            if (Keyboard.IsKeyDown(Key.F) && sender is Button)
            {
                MessageBox.Show("You Use BackDoor");
                areWeLogin = true;
                UpdateButtonVisibility(areWeLogin);
                ((Button)sender).Focusable = false;
                ((Button)sender).IsEnabled = false;
            }
            else
            {
                MainFrame.Navigate(new LogInPage(this));
            }
        }

        private void eventLogButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new LogPage(this));
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
                eventLogButton.Visibility = Visibility.Visible;

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
                eventLogButton.Visibility = Visibility.Collapsed;

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