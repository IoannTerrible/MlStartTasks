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
        #region Fields

        public static string connectionString = App.ContentFromConfig["ConnectionString"];
        public bool areWeLogin = false;
        public bool areWeConnected = false;
        public bool isServerAlive = false;

        public VideoPage activyVideoPage;
        public static ApiClient apiClient;

        public static HttpClient client = new();
        public static HealthChecker healthChecker;

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
            apiClient = new ApiClient(this);
            UpdateButtonVisibility(areWeLogin);
            string[] eventLogColumns =
            {
                "UserName VARCHAR(255) NULL",
                "FileName VARCHAR(255) NULL",
                "FramePath NVARCHAR(MAX) NULL",
                "MetaData NVARCHAR(MAX) NULL"
            };
            SqlCore.CreateTable(connectionString, "EventLog", eventLogColumns);
        }

        #endregion

        #region Event Handlers

        private async void FastConnectClick(object sender, RoutedEventArgs e)
        {
            ConnectionWindow.ConnectionUri = $"http://{App.ContentFromConfig["Ip"]}:{App.ContentFromConfig["Port"]}/";
            areWeConnected = true;
            UserStatus.Text = ConnectionWindow.ConnectionUri.ToString();
            healthChecker = new(this, statusTextBox);
            healthChecker.StartHealthCheckLoop(ConnectionWindow.ConnectionUri, 600);
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
                if (activyVideoPage == null)
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

        #endregion

        #region Methods
        public void UpdateButtonVisibility(bool areWeLoggedIn)
        {
            if (areWeLoggedIn)
            {
                ShowFunButtons();
                HideLogInButtons();
            }
            else
            {
                HideFunButtons();
                ShowLogInButtons();
            }
        }
        private void ShowFunButtons()
        {
            fastConnectButton.Visibility = Visibility.Visible;
            disconButton.Visibility = Visibility.Visible;
            imagePageButton.Visibility = Visibility.Visible;
            configButton.Visibility = Visibility.Visible;
            connectButton.Visibility = Visibility.Visible;
        }
        private void ShowLogInButtons()
        {
            loginButton.Visibility = Visibility.Visible;
            registrationButton.Visibility = Visibility.Visible;
        }
        private void HideLogInButtons()
        {
            loginButton.Visibility = Visibility.Collapsed;
            registrationButton.Visibility = Visibility.Collapsed;
        }
        private void HideFunButtons()
        {
            fastConnectButton.Visibility = Visibility.Collapsed;
            disconButton.Visibility = Visibility.Collapsed;
            imagePageButton.Visibility = Visibility.Collapsed;
            configButton.Visibility = Visibility.Collapsed;
            connectButton.Visibility = Visibility.Collapsed;
        }
        #endregion
    }
}
