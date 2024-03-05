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

        public Connector(string host, int port)
        {
            _host = host;
            _port = port;
        }
        Socket sender = new Socket(Dns.GetHostEntry("localhost").AddressList[0].AddressFamily, SocketType.Stream, ProtocolType.Tcp))
        public async Task SendMessage(string message)
        {

            {
                await sender.ConnectAsync(_host, _port);
                byte[] msg = Encoding.UTF8.GetBytes(message);
                await sender.SendAsync(msg);
            }
        }

        public async Task<string> ReceiveMessage()
        {
            byte[] bytes = new byte[1024];
            {
                await sender.ConnectAsync(_host, _port);
                int bytesRec = await sender.ReceiveAsync(new ArraySegment<byte>(bytes), SocketFlags.None);
                return Encoding.UTF8.GetString(bytes, 0, bytesRec);
            }
        }
    }
}
