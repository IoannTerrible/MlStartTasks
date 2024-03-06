using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SocketClient
{
    /// <summary>
    /// Логика взаимодействия для LogInPage.xaml
    /// </summary>
    public partial class LogInPage : Page
    {
        public MainWindow mainWindow;
        public static bool isWeAreConnect = false;
        public static bool isWeAreLogIn = false;
        public LogInPage(MainWindow _mainWindow)
        {
            mainWindow = _mainWindow;
            InitializeComponent();
        }
        private void enter_Click(object sender, RoutedEventArgs e)
        {
            if (!isWeAreConnect)
            {
                MessageBox.Show("NeedPressConnectButton");
                return;
            }
            if (isWeAreLogIn)
            {
                MessageBox.Show("Well you already logIn");
                return;
            }
            if (string.IsNullOrWhiteSpace(textBox_login.Text))
            {
                MessageBox.Show("Введите логин");
                return;
            }

            if (string.IsNullOrWhiteSpace(password.Password))
            {
                MessageBox.Show("Введите пароль");
                return;
            }
            string tempString = $"LOG {textBox_login.Text} {password.Password}";
            mainWindow.SendMessageAndReceive(tempString);
        }

        private void con_Click(object sender, RoutedEventArgs e)
        {
            mainWindow._socketClient.Connect();
            mainWindow.SendMessageAndReceive("CON");
            isWeAreConnect = true; 

        }
    }
}

