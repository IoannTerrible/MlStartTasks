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
        public ClassForAuth() { }
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
        public static string RegIn(string username, string password)
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
        private static DataTable ExecuteSqlCommand(SqlCommand sqlcom)
        {
            try
            {
                if (!DatabaseExists("MLstartDataBase"))
                {
                    Logger.LogByTemplate(LogEventLevel.Error, note:"DataBase not exists");
                    return null;
                }
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
                Logger.LogByTemplate(LogEventLevel.Error, ex, note: $"Error while work with table");
                return null;
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
                Console.WriteLine("Ошибка при проверке существования базы данных: " + ex.Message);
                return false;
            }
        }
    }
}
