using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Channels;
using System.IO;

namespace ServerLibrary
{

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class LoreServise : IServiseForServer
    {
        int nextId = 1;
        List<ServerUser> users = new List<ServerUser>();

        public void Connect(string connectlogin, string connectpassword)
        {
            ServerUser user = new ServerUser()
            {
                Id = nextId,
                Login = connectlogin,
                Password = connectpassword,
                OperContext = OperationContext.Current
            };
            nextId++;
            users.Add(user);
            string filePath = "errorlog.txt";
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine($"{user.Login} {user.Id} {user.OperContext}, {user.Password}");
            }

        }
        public void Disconnect(string connectlogin)
        {
            var user = users.Find(x => x.Login == connectlogin);
            if (user != null)
            {
                string tempString = $"{DateTime.Now} Bye {user.Login}";
                if (TestVoid(tempString))
                {
                    users.Remove(user);
                }
            }
        }
        bool TestVoid(string message)
        {
            SendStringMessage(message);
            return true;
        }
        public bool CheckHashAndLog(string chekingString, string login)
        {
            var user = users.Find(x => x.Login == login);
            if (user != null)
            {
                string hashPassword = GetHashString(chekingString);

                SqlCommand command = new SqlCommand();
                command.CommandText = $"SELECT COUNT(*) FROM [MLstartDataBase].[dbo].[Userss] WHERE [Login] = @Login AND [PassWord] = @Password";
                command.Parameters.AddWithValue("@Login", user.Login);
                command.Parameters.AddWithValue("@Password", hashPassword);
                DataTable dt_user = ExecuteSqlCommand(command);
                if (Convert.ToInt32(dt_user.Rows[0][0]) > 0)
                {
                    user.OperContext.GetCallbackChannel<IServiseForServerCallback>().DoYouLog(true);
                    return true;
                }
                else
                {
                    user.OperContext.GetCallbackChannel<IServiseForServerCallback>().DoYouLog(false);
                    return false;
                }
            }
            else
            {
                return false;
            }
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
                //Logger.LogByTemplate(Serilog.Events.LogEventLevel.Error, note: $"Error while work with table + {ex}");
                //Console.WriteLine("Error occurred: " + ex.Message);
                return null;
            }
        }
        public void SendStringMessage(string message)
        {
            foreach (var user in users)
            {
                if (user != null)
                {
                    user.OperContext.GetCallbackChannel<IServiseForServerCallback>().ReceiveLoreMessage(message);
                }
            }
        }
    }
}
