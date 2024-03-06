using System;
using System.Collections.Generic;
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
