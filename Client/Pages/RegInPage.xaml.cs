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
        private MainWindow _mainWindow;
        public RegInPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }
        private void enter_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxReg.Text) ||
                string.IsNullOrWhiteSpace(passwordForReg.Password) ||
                string.IsNullOrWhiteSpace(passwordCopy.Password))
            {
                MessageBox.Show("Enter Login, password and repeat password ");
                Logger.LogByTemplate(Serilog.Events.LogEventLevel.Warning, note: "Attempted registration with empty fields");
                return;
            }
            if (passwordForReg.Password != passwordCopy.Password)
            {
                MessageBox.Show("Password didn't match");
                Logger.LogByTemplate(Serilog.Events.LogEventLevel.Warning, note: "Password mismatch during registration for user " + textBoxReg.Text);
                return;
            }
            try
            {
                if (SqlCore.RegistrationIn(textBoxReg.Text, passwordForReg.Password, MainWindow.connectionString))
                {
                    MessageBox.Show("You Reg successfully, Now you can login");
                    Logger.LogByTemplate(Serilog.Events.LogEventLevel.Information, note: $"User {textBoxReg.Text} Reg");
                }
                else
                {
                    MessageBox.Show("Error while registration, check logs");
                    Logger.LogByTemplate(Serilog.Events.LogEventLevel.Error, note: "Registration failed for user " + textBoxReg.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred");
                Logger.LogByTemplate(Serilog.Events.LogEventLevel.Error, ex, "Exception during registration for user " + textBoxReg.Text);
            }
        }
    }
}
