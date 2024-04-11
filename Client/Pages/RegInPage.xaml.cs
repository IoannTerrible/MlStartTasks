using ClassLibrary;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для RegInPage.xaml
    /// </summary>
    public partial class RegInPage : Page
    {
        public MainWindow mainWindow;
        public RegInPage(MainWindow _mainWindow)
        {
            InitializeComponent();
            mainWindow = _mainWindow;
        }
        private void enter_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxReg.Text) ||
                string.IsNullOrWhiteSpace(passwordForReg.Password) ||
                string.IsNullOrWhiteSpace(passwordCopy.Password))
            {
                MessageBox.Show("Enter Login, password and repeat password");
                return;
            }
            if (passwordForReg.Password != passwordCopy.Password)
            {
                MessageBox.Show("Password didn't match");
                return;
            }
            if(SqlCore.RegistrationIn(textBoxReg.Text, passwordForReg.Password, MainWindow.connectionString))
            {
                MessageBox.Show("You Reg successfully, Now you can login");
            }
            else
            {
                MessageBox.Show("Eroor while registration, chek logs");
            }
        }
    }
}
