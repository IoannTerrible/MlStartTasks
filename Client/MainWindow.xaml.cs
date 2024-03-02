using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;
using Serilog.Events;

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

        public void LogAndConnect(string password, string login)
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
                client.ConnectAsync(login, password);
                //ResIp(login);
            }
            else
            {
                MessageBox.Show("YouNeedToConnect");
            }
        }

        public void RegAndConnectAndDisconnect(string username, string password)
        {
            client = new LoreServiseRef.ServiseForServerClient(new System.ServiceModel.InstanceContext(this));
            ClientLogger.LogByTemplate(LogEventLevel.Information, note: "Registration and connection process started.");
            client.RegInAsync(username, password);
            ClientLogger.LogByTemplate(LogEventLevel.Information, note: "Registration and connection process finish.");
            client = null;
        }

        private void GetIpButton_click(object sender, RoutedEventArgs e)
        {
            ResIp(YouLogin);
        }

        void ResIp(string log)
        {
            if (isConnected)
            {
                UserIpBox.Text = client.ResiveIp(log);
            }
            else
            {
                MessageBox.Show("You need to log and connect");
            }
        }

        public void SetIpInTextbox(string ip)
        {
            UserIpBox.Text = ip;
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
                MessageBox.Show("You are loged in server" + App.realContent[0]);
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
                try
                {
                    client.DisconnectAsync(YouLogin);
                    UpdateUIOnDisconnect();
                    ClientLogger.LogByTemplate(LogEventLevel.Information, note: "Disconnected from server.");
                    throw new Exception("TestError");
                }
                catch (Exception ex)
                {
                    ClientLogger.LogByTemplate(LogEventLevel.Error, ex, "Failed to disconnect from server.");
                    MessageBox.Show("Failed to disconnect from server.");
                }
            }
            else
            {
                ClientLogger.LogByTemplate(LogEventLevel.Warning, note: "Failed to disconnect. Not connected to server.");
                MessageBox.Show("You are not connected.");
            }
        }

        private void UpdateUIOnDisconnect()
        {
            UserNameTextBox.Text += " (Disconnected)";
            UserIpBox.Text = " Your IP was here:";
            client = null;
            isConnected = false;
            LogPage.isWeAreConnect = false;
            LogPage.isWeAreLogIn = false;
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
            MainFrame.Navigate(new RegPage(this));
        }

        private void StoryClick(object sender, RoutedEventArgs e)
        {
            if (isConnected)
            {
                MainFrame.Navigate(new StoryPage(this, new List<string>(client.ReciveLoreString()), client.ReciveDelay()));
            }
            else
            {
                MessageBox.Show("Sorry.Need to connect");
            }
        }
    }
}
