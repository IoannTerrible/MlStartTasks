//using ClassLibraryOne;
using ClassLibraryOne;
using MlStartTask2;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private App _app;

        //private readonly ClassLibraryOne.UiAndMainConnector _connector;

        public MainWindow()
        {
            InitializeComponent();
            OpenPage(Pages.login);
            _app = (App)Application.Current;
            //_connector = new UiAndMainConnector();
            //_connector.LinesUpdated += Connector_LinesUpdated;
            //List<String> storylinesfromui = _connector.freshLines;
            //Thread secondThread = new Thread(StartProcessingLines);
            //secondThread.Start();
        }
        public async void StartProcessingLines(List<string> storyLinesFromApp, int delayInSeconds)
        {
            await Task.Run(async () =>
            {
                foreach (var line in storyLinesFromApp)
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        StoryListBox.Items.Add(line);
                    }, DispatcherPriority.Background);
                    await Task.Delay(delayInSeconds);
                }
            });
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //StartProcessingLines();
        }
        public enum Pages
        {
            login,
            regin
        }
        //private void Connector_LinesUpdated(object sender, List<string> lines)
        //{
        //    // Обновляем ListBox новыми строками

        //    lines.Add("Hello, Windows Presentation Foundation!");
        //}

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
                ClassLibraryOne.Logger.LogByTemplate(Serilog.Events.LogEventLevel.Error, note: "Error while SelectTable");
                Console.WriteLine("Error occurred: " + ex.Message);
                return null;
            }
        }

    }
}