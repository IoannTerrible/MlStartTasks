using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;

namespace SocketClient
{
    public class Connector
    {
        private readonly string _host;
        private readonly int _port;
        private Socket sender;
        public bool IsConnected = false;
        public Connector(string host, int port)
        {
            _host = host;
            _port = port;
        }
        public async void Connect()
        {
            IPAddress ipAddress = (await Dns.GetHostEntryAsync(_host)).AddressList[0];
            sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            IsConnected = true;
            await sender.ConnectAsync(ipAddress, _port);
        }
        
        public async Task SendMessage(string message)
        {
            if (IsConnected)
            {
                {
                    byte[] msg = Encoding.UTF8.GetBytes(message);
                    await sender.SendAsync(msg);
                }
            }
        }

        public async Task<string> ReceiveMessage()
        {
            if (IsConnected)
            {
                byte[] bytes = new byte[1024];
                {
                    int bytesRec = await sender.ReceiveAsync(new ArraySegment<byte>(bytes), SocketFlags.None);
                    return Encoding.UTF8.GetString(bytes, 0, bytesRec);
                }
            }
            else
            {
                return null;
            }
        }
        public void Disconnect()
        {
            if (IsConnected && sender != null)
            {
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                IsConnected = false;
            }
        }
    }
}
