using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для LogPage.xaml
    /// </summary>
    public partial class LogPage : Page
    {
        public MainWindow mainWindow;
        public static bool isWeAreConnect = false;
        public static bool isWeAreLogIn = false;

        public LogPage()
        {
            InitializeComponent();
        }

        public LogPage(MainWindow _mainWindow) : this() 
        {
            mainWindow = _mainWindow;
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
            mainWindow.LogInServer(password.Password,textBox_login.Text);
            isWeAreLogIn = true;
        }

        private void con_Click(object sender, RoutedEventArgs e)
        {
            if (isWeAreConnect)
            {
                MessageBox.Show("Better don't do this while you already connect (Disconnect)");
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
            isWeAreConnect = true;
            mainWindow.LogAndConnect(password.Password, textBox_login.Text);
        }
    }
}
