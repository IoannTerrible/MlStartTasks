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
        LoreServiseRef.ServiseForServerClient client;
        public MainWindow()
        {
            InitializeComponent();
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
                //client.Connect(UserNameTextBox.Text, password.Password);
                isConnected = true;
            }
            else
            {
                MessageBox.Show($"Seems you already connected to {client}");
            }

        }
        //public DataTable SendRquestToExecuteSql(SqlCommand command)
        //{
        //    client
        //}
        private void DisconnectClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("HelpMe");
        }

        private void LogClick(object sender, RoutedEventArgs e)
        {

        }

        private void RegClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new LogPage(this));
        }

    }
}
