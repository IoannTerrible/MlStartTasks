using System.Data.SqlClient;
using System.Data;
using System;
using System.Windows;
using System.Threading.Tasks;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// //Client.ServiseLoreReference
    public partial class MainWindow : Window, LoreServiseRef.IServiseForServerCallback
    {
        bool isConnected = false;
        bool isLoginServer = false;
        string YouLogin;
        LoreServiseRef.ServiseForServerClient client;
        public MainWindow()
        {
            InitializeComponent();
        }
        public void TrueConnect(string password, string login)
        {
            if (!isConnected)
            {
                client = new LoreServiseRef.ServiseForServerClient(new System.ServiceModel.InstanceContext(this));
                isConnected = true;
                UserStatus.Text = $"You are connected to {client}";
            }
            if (isConnected)
            {
                UserNameTextBox.Text = login;
                YouLogin = login;
                client.Connect(login, password);
            }
            else
            {
                MessageBox.Show("YouNeedToConnect");
            }
        }
        public void LogInServer(string password, string login)
        {
            if (isConnected)
            {
                client.CheckHashAndLog(password, login);
            }
            else
            {
                MessageBox.Show("You need to connect");
            }
        }
        public void DoYouLog(bool IsLogin)
        {
            isLoginServer = IsLogin;
            if (isLoginServer)
            {
                MessageBox.Show("You are loged in server");
            }
            else
            {
                MessageBox.Show("You are not loged in server");
            }
        }
        public void ReceiveLoreMessage(string message)
        {
            UserStatus.Text = message;
        }
        private void DisconnectClick(object sender, RoutedEventArgs e)
        {
            Disconnect();
        }
        void Disconnect()
        {
            if (isConnected)
            {
                client.DisconnectAsync(YouLogin);
                client = null;
                isConnected = false;
                LogPage.isWeAreConnect = false;
                LogPage.isWeAreLogIn = false;
            }
            else
            {
                MessageBox.Show("You are not coonnndecer");
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Disconnect();
        }

        private void LogClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new LogPage(this));
        }

        private void RegClick(object sender, RoutedEventArgs e)
        {

        }

    }
}
