using MlStartTask2;
using Serilog.Core;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
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
        public static string GetHashString(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
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
        public DataTable ExecuteSqlCommand(string sqlString)
        {
            try
            {
                DataTable dataTable = new DataTable("dataBase");
                using (SqlConnection sqlConnection = new SqlConnection("server=(localdb)\\MSSqlLocalDb;Trusted_Connection=Yes;DataBase=MLstartDataBase;"))
                {
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(sqlString, sqlConnection);
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