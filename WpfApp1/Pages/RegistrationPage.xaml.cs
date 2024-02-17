using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        public MainWindow mainWindow;
        public RegistrationPage(MainWindow _mainWindow)
        {
            InitializeComponent();
            mainWindow = _mainWindow;
        }
        private void cancer_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.OpenPage(MainWindow.Pages.login);
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

            SqlCommand command = new SqlCommand();
            command.CommandText = $"INSERT INTO [MLstartDataBase].[dbo].[Userss] (Login, PassWord) VALUES (@Login, @Password)";
            command.Parameters.AddWithValue("@Login", textBoxReg.Text);
            command.Parameters.AddWithValue("@Password", ClassLibraryOne.Hasher.GetHashString(passwordForReg.Password));
            mainWindow.ExecuteSqlCommand(command);
            MessageBox.Show("Регистрация прошла успешно");
            mainWindow.OpenPage(MainWindow.Pages.login);
        }
    }
}
