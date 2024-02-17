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
        public DataTable ExecuteSqlCommand(SqlCommand sqlcom)
        {
            try
            {
                DataTable dataTable = new DataTable("dataBase");
                using (SqlConnection sqlConnection = new SqlConnection("server=(localdb)\\MSSqlLocalDb;Trusted_Connection=Yes;DataBase=MLstartDataBase;"))
                {
                    sqlConnection.Open();
                    sqlcom.Connection = sqlConnection;
                    SqlDataAdapter adapter = new SqlDataAdapter(sqlcom); 
                    adapter.Fill(dataTable);
                }
                return dataTable;
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(Serilog.Events.LogEventLevel.Error, note: $"Error while work with table + {ex}");
                //Console.WriteLine("Error occurred: " + ex.Message);
                return null;
            }
        }

    }
}