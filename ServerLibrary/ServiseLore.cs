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
                OperContext = OperationContext.Current,
                realIp = (OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty).Address
            };
            nextId++;
            users.Add(user);
            Logger.LogByTemplate(Serilog.Events.LogEventLevel.Information, note: $"New Client added {user}");
        }
        public void Disconnect(string connectlogin)
        {
            var user = users.Find(x => x.Login == connectlogin);
            if (user != null)
            {
                SendStringMessage($"Bye {user.Login}");
                users.Remove(user);
            }
        }
        public void CheckHashAndLog(string chekingString, string login)
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
                }
                else
                {
                    user.OperContext.GetCallbackChannel<IServiseForServerCallback>().DoYouLog(false);
                }
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
        public void RegIn(string username, string password)
        {
            try
            {
                SqlCommand command = new SqlCommand();
                command.CommandText = $"INSERT INTO [MLstartDataBase].[dbo].[Userss] (Login, PassWord) VALUES (@Login, @Password)";
                command.Parameters.AddWithValue("@Login", username);
                command.Parameters.AddWithValue("@Password", GetHashString(password));
                ExecuteSqlCommand(command);
            }
            catch (Exception ex)
            {
                SendStringMessage("Sorry" + ex.Message);
            }
        }

        public string ResiveIp(string login)
        {
            var user = users.Find(x => x.Login == login);
            if (user != null)
            {
                try
                {
                    return user.realIp.ToString();
                }
                catch (Exception ex)
                {
                    //user.OperContext.GetCallbackChannel<IServiseForServerCallback>().SetIpInTextbox("Error " + ex.Message);
                    return ("Error " + ex.Message);
                }
            }
            return "User is null";
        }
        public void ReciveConfigData(string[] dataFromConfig)
        {
            try
            {
                MainFunProgram.GetNumbersFromSendedArrayOfStrings(dataFromConfig);
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(Serilog.Events.LogEventLevel.Error, ex, note: "Error trying to call ReciveConfigData");
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
