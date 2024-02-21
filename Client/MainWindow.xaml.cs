using System.Data.SqlClient;
using System.Data;
using System;
using System.Windows;

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
        LoreServiseRef.ServiseForServerClient client;
        public MainWindow()
        {
            InitializeComponent();
        }
        public void TrueConnect(string password, string login)
        {
            if (isConnected)
            {
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
                
                isLoginServer = client.CheckHashAndLog(password, login);
                if (isLoginServer)
                {
                    MessageBox.Show("You are loged in server");
                }
                if (!isLoginServer)
                {
                    MessageBox.Show("You are not loged in server");
                }
            }
            else
            {
                MessageBox.Show("You need to connect");
            }
        }
        public bool DoYouLog(bool IsLogin)
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
            return true;
        }
        public void ReceiveLoreMessage(string message)
        {
            UserStatus.Text = message;
        }

        private void ConnectClick(object sender, RoutedEventArgs e)
        {
            if (!isConnected)
            {
                client = new LoreServiseRef.ServiseForServerClient(new System.ServiceModel.InstanceContext(this));
                client.Connect("weeww", "wedse");
                isConnected = true;
            }
            else
            {
                MessageBox.Show($"Seems you already connected to {client}");
            }

        }
        private void DisconnectClick(object sender, RoutedEventArgs e)
        {
            Disconnect();
        }
        void Disconnect()
        {
            if (isConnected)
            {
                client.Disconnect("weeww");
                client = null;
                isConnected = false;
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
            client.SendStringMessage("Text");
        }

        private void RegClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new LogPage(this));
        }

    }
}
