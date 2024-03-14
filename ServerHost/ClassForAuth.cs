using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary;
using Serilog.Events;

namespace ServerHost
{
    internal class ClassForAuth
    {
        #region Fields
        readonly static string connectionStringFromConfig = Program.ContentFromServerConfig[5];
        #endregion
        #region Public Methods
        public static bool CheckHashAndLog(string login, string chekingString)
        {
            {
                string hashPassword = GetHashString(chekingString);

                SqlCommand command = new SqlCommand();
                command.CommandText = $"SELECT COUNT(*) FROM [MLstartDataBase].[dbo].[Userss] WHERE [Login] = @Login AND [PassWord] = @Password";
                command.Parameters.AddWithValue("@Login", login);
                command.Parameters.AddWithValue("@Password", hashPassword);
                DataTable dt_user = ExecuteSqlCommand(command);
                if (Convert.ToInt32(dt_user.Rows[0][0]) > 0)
                {
                    return (true);
                }
                else
                {
                    return (false);
                }
            }
        }
        public static string RegistrationIn(string username, string password)
        {
            try
            {
                SqlCommand command = new SqlCommand();
                command.CommandText = $"INSERT INTO [MLstartDataBase].[dbo].[Userss] (Login, PassWord) VALUES (@Login, @Password)";
                command.Parameters.AddWithValue("@Login", username);
                command.Parameters.AddWithValue("@Password", GetHashString(password));
                ExecuteSqlCommand(command);
                return "You Successfully Reg";
            }
            catch (Exception ex)
            {
                return ("Sorry" + ex.Message);
            }
        }
        #endregion
        #region Private Methods
        private static string GetHashString(string input)
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
        private static DataTable ExecuteSqlCommand(SqlCommand sqlcom)
        {
            try
            {
                if (!DatabaseExists("MLstartDataBase"))
                {
                    Logger.LogByTemplate(LogEventLevel.Error, note: "DataBase not exists");
                    CreateDatabase();
                }
                if (!TableExists("MLstartDataBase", "Userss"))
                {
                    Logger.LogByTemplate(LogEventLevel.Error, note: "Table Userss does not exist");
                    CreateTableUserss();
                }
                DataTable dataTable = new DataTable();
                using (SqlConnection sqlConnection = new SqlConnection(connectionStringFromConfig))
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
                Logger.LogByTemplate(LogEventLevel.Error, ex, note: $"Error while work with table");
                return null;
            }
        }
        private static bool TableExists(string databaseName, string tableName)
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
        private static bool DatabaseExists(string databaseName)
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
        private static void CreateDatabase()
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

        private static void CreateTableUserss()
        {
            try
            {
                Logger.LogByTemplate(LogEventLevel.Information, null, "Creating Userss table...");
                using (SqlConnection databaseConnection = new SqlConnection(connectionStringFromConfig))
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
        #endregion
    }
}
