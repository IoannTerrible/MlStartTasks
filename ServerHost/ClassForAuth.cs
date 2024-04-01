using ClassLibrary;
using Serilog.Events;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

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

                SqlCommand command = new();
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
                SqlCommand command = new();
                command.CommandText = $"INSERT INTO [MLstartDataBase].[dbo].[Userss] (Login, PassWord) VALUES (@Login, @Password)";
                command.Parameters.AddWithValue("@Login", username);
                command.Parameters.AddWithValue("@Password", GetHashString(password));
                ExecuteSqlCommand(command);
                return "You Successfully RegistrationIn";
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
                if (!SqlCore.DatabaseExists("MLstartDataBase"))
                {
                    Logger.LogByTemplate(LogEventLevel.Error, note: "DataBase not exists");
                    SqlCore.CreateDatabase();
                }
                if (!SqlCore.TableExists("MLstartDataBase", "Userss"))
                {
                    Logger.LogByTemplate(LogEventLevel.Error, note: "Table Userss does not exist");
                    SqlCore.CreateTableUserss(connectionStringFromConfig);
                }
                return SqlCore.ExecuteSQL(sqlcom, connectionStringFromConfig);
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(LogEventLevel.Error, ex, note: $"Error while work with table");
                return null;
            }
        }
        
        #endregion
    }
}
