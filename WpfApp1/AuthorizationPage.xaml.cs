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
            if (textBox_login.Text.Length > 0)    
            {
                if (password.Password.Length > 0)          
                {
                    DataTable dt_user = mainWindow.Select($"SELECT * FROM [MLstartDataBase].[dbo].[Userss] WHERE [Login] = '{textBox_login.Text}' AND [PassWord] = '{password.Password}'");
                    if (dt_user.Rows.Count > 0)       
                    {
                        MessageBox.Show("Пользователь авторизовался");          
                    }
                    else MessageBox.Show("Пользователя не найден"); 
                }
                else MessageBox.Show("Введите пароль"); 
            }
            else MessageBox.Show("Введите логин");
        }
        private void cancer_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.OpenPage(MainWindow.Pages.regin);
        }
    }
}
