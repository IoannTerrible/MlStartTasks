using ClassLibraryOne;
using MlStartTask2;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private App _app;
        public MainWindow()
        {
            InitializeComponent();
            OpenPage(Pages.login);
            _app = (App)Application.Current;


        }


        private void button_Click(object sender, RoutedEventArgs e)
        {
            DebugTextBox.Text = $"{Program.lines}";
        }
        public enum Pages
        {
            login,
            regin,
            storyline
        }

        public void OpenPage(Pages pages)
        {
            if (pages == Pages.login)
            {
                MainFrame.Navigate(new AuthorizationPage(this));
            }
            if (pages == Pages.regin)
            {
                MainFrame.Navigate(new RegistrationPage(this));
            }
            if (pages == Pages.storyline)
            {
                MainFrame.Navigate(new StoryLinePage(this));
            }
        }
        

    }
}