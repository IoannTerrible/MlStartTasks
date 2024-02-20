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

        public LogPage(MainWindow _mainWindow)
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
            //string hashPassword = ClassLibraryOne.Hasher.GetHashString(password.Password);

            //SqlCommand command = new SqlCommand();
            //command.CommandText = $"SELECT COUNT(*) FROM [MLstartDataBase].[dbo].[Userss] WHERE [Login] = @Login AND [PassWord] = @Password";
            //command.Parameters.AddWithValue("@Login", textBox_login.Text);
            //command.Parameters.AddWithValue("@Password", hashPassword);
            //DataTable dt_user = mainWindow.SendRquestToExecuteSql(command);
            //if (Convert.ToInt32(dt_user.Rows[0][0]) > 0)
            //{
            //    MessageBox.Show("Пользователь авторизовался");
            //    //mainWindow.OpenPage(MainWindow.Pages.storyline);
            //}
            else
            {
                MessageBox.Show("Пользователя не найден");
            }
        }
        private void cancer_Click(object sender, RoutedEventArgs e)
        {
            //mainWindow.OpenPage(MainWindow.Pages.regin);
        }
    }
}
