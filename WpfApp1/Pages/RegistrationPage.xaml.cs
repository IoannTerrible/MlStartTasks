using System.Data;
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
            if (string.IsNullOrWhiteSpace(textBoxReg.Text))
            {
                MessageBox.Show("Введите логин");
                return;
            }

            if (string.IsNullOrWhiteSpace(passwordForReg.Password))
            {
                MessageBox.Show("Введите пароль");
                return;
            }

            if (string.IsNullOrWhiteSpace(passwordCopy.Password))
            {
                MessageBox.Show("Повторите пароль");
                return;
            }
            else
            {
                mainWindow.ExecuteSqlCommand("INSERT INTO Userss (Login, PassWord) VALUES ('" + textBoxReg.Text + "', '" + ClassLibraryOne.Hasher.GetHashString(passwordForReg.Password) + "')") ;
                mainWindow.OpenPage(MainWindow.Pages.storyline);
            }

        }
    }
}
