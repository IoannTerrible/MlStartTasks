using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ServerLibrary
{
    // ПРИМЕЧАНИЕ. Можно использовать команду "Переименовать" в меню "Рефакторинг", чтобы изменить имя интерфейса "IService1" в коде и файле конфигурации.
    [ServiceContract(CallbackContract = typeof(IServiseForServerCallback))]
    public interface IServiseForServer
    {
        [OperationContract]
        void Connect(string connectlogin, string connectpassword);

        [OperationContract]
        void Disconnect(int userId);
        [OperationContract]
        void SendStringMessage(string message);
        [OperationContract]
        void CheckHash(string chekingString);
        //[OperationContract]
        //DataTable ExecuteSqlCommand(SqlCommand sqlcom);
    }
    public interface IServiseForServerCallback
    {
        [OperationContract(IsOneWay = true)]
        void ReceiveLoreMessage(string message);
        [OperationContract(IsOneWay = true)]
        void LogInServer(bool result);
    }
}
