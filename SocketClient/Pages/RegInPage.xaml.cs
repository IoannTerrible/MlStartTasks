using System.Windows;
using System.Windows.Controls;

namespace SocketClient
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
            string tempString = $"REG {textBoxReg.Text} {passwordForReg.Password}";
            mainWindow.SendMessageAndReceive(tempString);
        }
    }
}
