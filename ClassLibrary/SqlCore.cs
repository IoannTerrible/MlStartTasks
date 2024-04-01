using Serilog.Events;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;


namespace ClassLibrary
{
    public class SqlCore
    {
        public static void CreateDatabase()
        {
            try
            {
                Logger.LogByTemplate(LogEventLevel.Information, null, "Creating database...");
                using (SqlConnection masterConnection = new SqlConnection("server=(localdb)\\MSSqlLocalDb;Trusted_Connection=Yes;"))
                {
                    masterConnection.Open();
                    SqlCommand createDbCommand = new SqlCommand("CREATE DATABASE MLstartDataBase", masterConnection);
                    createDbCommand.ExecuteNonQuery();
                    Logger.LogByTemplate(LogEventLevel.Information, null, "Database created successfully.");
                }
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(LogEventLevel.Error, ex, "Error creating database.");
            }
        }

        public static void CreateTableUserss(string connectionString)
        {
            try
            {
                Logger.LogByTemplate(LogEventLevel.Information, null, "Creating Userss table...");
                using (SqlConnection databaseConnection = new SqlConnection(connectionString))
                {
                    databaseConnection.Open();
                    SqlCommand createTableCommand = new SqlCommand(@"
                    CREATE TABLE Userss (
                    Personid INT PRIMARY KEY IDENTITY,
                    Login VARCHAR(255) NOT NULL,
                    PassWord VARCHAR(255) NOT NULL
                    )", databaseConnection);
                    createTableCommand.ExecuteNonQuery();
                    Logger.LogByTemplate(LogEventLevel.Information, null, "Userss table created successfully.");
                }
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(LogEventLevel.Error, ex, "Error creating Userss table.");
            }
        }
        public static bool TableExists(string databaseName, string tableName)
        {
            using (SqlConnection databaseConnection = new SqlConnection($"server=(localdb)\\MSSqlLocalDb;Trusted_Connection=Yes;DataBase={databaseName};"))
            {
                databaseConnection.Open();
                SqlCommand command = new SqlCommand($"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = '{tableName}'", databaseConnection);
                int result = (int)command.ExecuteScalar();
                if (result > 0)
                {
                    Logger.LogByTemplate(LogEventLevel.Information, note: $"Table {tableName} exists in database {databaseName}");
                    return true;
                }
                else
                {
                    Logger.LogByTemplate(LogEventLevel.Information, note: $"Table {tableName} does not exist in database {databaseName}");
                    return false;
                }
            }
        }
        public static bool DatabaseExists(string databaseName)
        {
            try
            {
                SqlCommand command = new SqlCommand();
                command.CommandText = "SELECT COUNT(*) FROM master.dbo.sysdatabases WHERE name = @DatabaseName";
                command.Parameters.AddWithValue("@DatabaseName", databaseName);

                using (SqlConnection connection = new SqlConnection("server=(localdb)\\MSSqlLocalDb;Trusted_Connection=True;"))
                {
                    connection.Open();
                    command.Connection = connection;
                    string databaseFilePath = GetDatabaseFilePath(connection, databaseName);
                    LogDatabaseFilePath(databaseFilePath);
                    int databaseCount = Convert.ToInt32(command.ExecuteScalar());
                    return databaseCount > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DataBase Chek error" + ex.Message);
                return false;
            }
        }
        public static string GetDatabaseFilePath(SqlConnection connection, string databaseName)
        {
            string query = $"SELECT physical_name FROM sys.master_files WHERE database_id = DB_ID('{databaseName}')";
            SqlCommand command = new SqlCommand(query, connection);
            return command.ExecuteScalar()?.ToString();
        }
        public static DataTable ExecuteSQL(SqlCommand sqlcom, string sqlConnectionString)
        {
            //Don't Try cath right here. Only where you summon this void
            DataTable dataTable = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(sqlConnectionString))
            {
                sqlConnection.Open();
                sqlcom.Connection = sqlConnection;
                SqlDataAdapter adapter = new SqlDataAdapter(sqlcom);
                adapter.Fill(dataTable);
            }
            return dataTable;
        }
        public static void LogDatabaseFilePath(string databaseFilePath)
        {
            if (string.IsNullOrEmpty(databaseFilePath))
            {
                Console.WriteLine("Error: Database file path is empty or null.");
                return;
            }

            string drive = Path.GetPathRoot(databaseFilePath);
            string folder = Path.GetDirectoryName(databaseFilePath)?.Replace(drive, "").Trim('\\');
            Logger.LogByTemplate(LogEventLevel.Information, note: $"Database file path: {drive} {folder}");
        }

    }
}
