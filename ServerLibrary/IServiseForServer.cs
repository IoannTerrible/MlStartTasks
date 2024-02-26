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

        [OperationContract(IsOneWay = true)]
        void Disconnect(string connectlogin);
        [OperationContract(IsOneWay = true)]
        void SendStringMessage(string message);
        [OperationContract(IsOneWay = true)]
        void CheckHashAndLog(string chekingString, string login);
        [OperationContract(IsOneWay = true)]
        void RegIn(string username, string password);
        [OperationContract()]
        string ResiveIp(string login);
    }
    public interface IServiseForServerCallback
    {
        [OperationContract(IsOneWay = true)]
        void ReceiveLoreMessage(string message);
        [OperationContract(IsOneWay = true)]
        void DoYouLog(bool IsLoggin);
        [OperationContract(IsOneWay = true)]
        void SetIpInTextbox(string ip);
    }
}
