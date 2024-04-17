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
        public bool isServerAlive = false;

        public ImagePage activyImagePage;
        public VideoPage activyVideoPage;
        public static ApiClient apiClient;

        public static HttpClient client = new();
        public static HealthChecker healthChecker;

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

            //ConnectionWindow.ConnectionUri = @"http://localhost:7000/";
            ConnectionWindow.ConnectionUri = $"http://{App.ContentFromConfig["Ip"]}:{App.ContentFromConfig["Port"]}/";
            areWeConnected = true;
            UserStatus.Text = ConnectionWindow.ConnectionUri.ToString();
            healthChecker = new(this, statusTextBox);
            healthChecker.StartHealthCheckLoop(ConnectionWindow.ConnectionUri, 2);

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
                if(activyImagePage == null)
                {
                    activyVideoPage = new VideoPage(this);
                }
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
    
    }
}