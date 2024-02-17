using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ServerLibrary
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "Service1" в коде и файле конфигурации.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Service1 : IServiseForServer
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

        public void DoWork()
        {
        }

        public void SendStringMessage(string message)
        {
            string responce = "";
            foreach (var user in users)
            {
                user.OperContext.GetCallbackChannel<IServerCallback>().ReceiveLoreMessage(responce += message);
            }
        }
    }
}
