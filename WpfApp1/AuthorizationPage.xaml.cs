using System.Data;
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
            string hashPassword = MainWindow.GetHashString(password.Password);
            DataTable dt_user = mainWindow.ExecuteSqlCommand(sqlString: $"SELECT COUNT(*) FROM [MLstartDataBase].[dbo].[Userss] WHERE [Login] = '{textBox_login.Text}' AND [PassWord] = '{hashPassword}'");
            if (Convert.ToInt32(dt_user.Rows[0][0]) > 0)
            {
                MessageBox.Show("Пользователь авторизовался");
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
