using Client;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegPage : Page
    {
        public MainWindow mainWindow;
        public RegPage(MainWindow _mainWindow)
        {
            InitializeComponent();
            mainWindow = _mainWindow;
        }
        private void enter_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxReg.Text) || string.IsNullOrWhiteSpace(passwordForReg.Password) || string.IsNullOrWhiteSpace(passwordCopy.Password))
            {
                MessageBox.Show("Введите логин, пароль и повторите пароль");
                return;
            }

            if (passwordForReg.Password != passwordCopy.Password)
            {
                MessageBox.Show("Пароли не совпадают");
                return;
            }
            mainWindow.RegAndConnectAndDisconnect(textBoxReg.Text, passwordForReg.Password);
            MessageBox.Show("Регистрация прошла успешно");
        }
    }
}
