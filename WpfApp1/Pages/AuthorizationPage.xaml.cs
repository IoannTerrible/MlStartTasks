using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationPage.xaml
    /// </summary>
    public partial class AuthorizationPage : Page   
    {
        public MainWindow mainWindow;

        public AuthorizationPage(MainWindow _mainWindow)
        {
            InitializeComponent();
            mainWindow = _mainWindow;
        }
        private void enter_Click(object sender, RoutedEventArgs e)
        {
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
            string hashPassword = ClassLibraryOne.Hasher.GetHashString(password.Password);
            
            SqlCommand command = new SqlCommand();
            command.CommandText = $"SELECT COUNT(*) FROM [MLstartDataBase].[dbo].[Userss] WHERE [Login] = @Login AND [PassWord] = @Password";
            command.Parameters.AddWithValue("@Login", textBox_login.Text);
            command.Parameters.AddWithValue("@Password", hashPassword);
            DataTable dt_user = mainWindow.ExecuteSqlCommand(command);
            if (Convert.ToInt32(dt_user.Rows[0][0]) > 0)
            {
                MessageBox.Show("Пользователь авторизовался");
                mainWindow.OpenPage(MainWindow.Pages.storyline);
            }
            else
            {
                MessageBox.Show("Пользователя не найден");
            }
        }
        private void cancer_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.OpenPage(MainWindow.Pages.regin);
        }
    }
}
