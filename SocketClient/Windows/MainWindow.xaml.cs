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
            activyImagePage = new ImagePage(this);
            MainFrame.Navigate(activyImagePage);
        }
        private async void ConfigClick(object sender, RoutedEventArgs e)
        {
            // open configuration
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