using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using MlStartTask2;
using Serilog.Core;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();           
            OpenPage(Pages.login);
        }
        public enum Pages
        {
            login,
            regin
        }
        void button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Hello, Windows Presentation Foundation!");
        }
        public void OpenPage(Pages pages)
        {
            if (pages == Pages.login)
            {
                MainFrame.Navigate(new AuthorizationPage(this));
            }
            if(pages == Pages.regin)
            {
                MainFrame.Navigate(new RegistrationPage(this));
            }
        }
        public DataTable Select(string selectSQL)
        {
            try
            {
                DataTable dataTable = new DataTable("dataBase");
                using (SqlConnection sqlConnection = new SqlConnection("server=(localdb)\\MSSqlLocalDb;Trusted_Connection=Yes;DataBase=MLstartDataBase;"))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(selectSQL, sqlConnection);
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    sqlDataAdapter.Fill(dataTable); 
                }
                return dataTable; 
            }
            catch (Exception ex)
            {
                MlStartTask2.Logger.LogByTemplate(Serilog.Events.LogEventLevel.Error, note: "Error while SelectTable");
                Console.WriteLine("Error occurred: " + ex.Message);
                return null; 
            }
        }
    
    }
}