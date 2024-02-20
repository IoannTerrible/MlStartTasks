using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;

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
                Password = connectpassword
            };
            nextId++;
            SendStringMessage(DateTime.Now.ToString() + " Hello " + user.Login);
            users.Add(user);
        }
        public void Disconnect(int userId)
        {
            var user = users.Find(x => x.Id == userId);
            if (user != null)
            {
                users.Remove(user);
                SendStringMessage(DateTime.Now.ToString() + " Bye " + user.Login);
            }
        }

        public void CheckHash(string chekingString)
        {

        }
        //public static string GetHashString(string input)
        //{
        //    using (SHA256 sha256Hash = SHA256.Create())
        //    {
        //        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

        //        StringBuilder builder = new StringBuilder();
        //        for (int i = 0; i < bytes.Length; i++)
        //        {
        //            builder.Append(bytes[i].ToString("x2"));
        //        }
        //        return builder.ToString();
        //    }
        //}
        //public DataTable ExecuteSqlCommand(SqlCommand sqlcom)
        //{
        //    try
        //    {
        //        DataTable dataTable = new DataTable("dataBase");
        //        using (SqlConnection sqlConnection = new SqlConnection("server=(localdb)\\MSSqlLocalDb;Trusted_Connection=Yes;DataBase=MLstartDataBase;"))
        //        {
        //            sqlConnection.Open();
        //            sqlcom.Connection = sqlConnection;
        //            SqlDataAdapter adapter = new SqlDataAdapter(sqlcom);
        //            adapter.Fill(dataTable);
        //        }
        //        return dataTable;
        //    }
        //    catch (Exception ex)
        //    {
        //        //Logger.LogByTemplate(Serilog.Events.LogEventLevel.Error, note: $"Error while work with table + {ex}");
        //        //Console.WriteLine("Error occurred: " + ex.Message);
        //        return null;
        //    }
        //}
        public void SendStringMessage(string message)
        {
            foreach (var user in users)
            {
                user.OperContext.GetCallbackChannel<IServiseForServerCallback>().ReceiveLoreMessage(message);
            }
        }
    }
}
