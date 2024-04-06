using System.Windows;
using System.Windows.Controls;

namespace SocketClient
{
    /// <summary>
    /// Логика взаимодействия для LogInPage.xaml
    /// </summary>
    public partial class LogInPage : Page
    {
        public MainWindow mainWindow;
        public static bool isWeAreLogIn = false;
        public static string? login;
        public LogInPage(MainWindow _mainWindow)
        {
            mainWindow = _mainWindow;
            InitializeComponent();
        }
        private void enter_Click(object sender, RoutedEventArgs e)
        {
            //if (!mainWindow.isConnected)
            //{
            //    MessageBox.Show("NeedPressConnectButton");
            //    return;
            //}
            //if (isWeAreLogIn)
            //{
            //    MessageBox.Show("Well you already logIn");
            //    return;
            //}
            //if (string.IsNullOrWhiteSpace(textBox_login.Text))
            //{
            //    MessageBox.Show("Enter login");
            //    return;
            //}

            //if (string.IsNullOrWhiteSpace(password.Password))
            //{
            //    MessageBox.Show("Enter password");
            //    return;
            //}
            //login = textBox_login.Text;
            //string tempString = $"LOG {textBox_login.Text} {password.Password}";
            //mainWindow.SendMessageAndReceive(tempString);
        }

    }
}

