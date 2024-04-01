using ClassLibrary;
using Serilog.Events;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketClient
{
    public class Connector
    {
        public readonly string _host;
        private readonly int _port;
        public Socket sender;
        private MainWindow _mainwindow;
        public bool IsConnected = false;
        public Connector(string host, int port, MainWindow mainWin)
        {
            _mainwindow = mainWin;
            _host = host;
            _port = port;
        }
        public async Task Connect()
        {
            try
            {
                Logger.LogByTemplate(LogEventLevel.Information, note: "Start connect in connector");
                IPAddress ipAddress = (await Dns.GetHostEntryAsync(_host)).AddressList[0];
                sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                IsConnected = true;
                sender.Connect(ipAddress, _port);
                Logger.LogByTemplate(LogEventLevel.Information, note: "End Connect in connector");
            }
            catch (SocketException ex) when (ex.Message.Contains("Этот хост неизвестен"))
            {
                Logger.LogByTemplate(LogEventLevel.Error, ex, note: $"Error: Host unknown {_host} {_port}");
            }
            catch (SocketException ex)
            {
                Logger.LogByTemplate(LogEventLevel.Error, ex, note: $"SocketError while connect to {_host} {_port}");
            }

            catch (Exception ex)
            {
                Logger.LogByTemplate(LogEventLevel.Error, ex, note: $"Error while connect to {_host} {_port}");
            }
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
        public async Task ReceiveLoreMessages()
        {
            byte[] buffer = new byte[1024];
            while (true)
            {
                try
                {
                    int bytesRead = await sender.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                    string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    if (receivedData == "S")
                    {
                        _mainwindow.SetButtonToGo(true);
                        _mainwindow.SetStopRecivingLines();
                        Logger.LogByTemplate(LogEventLevel.Debug, note: "Help me");
                        break;
                    }
                    await _mainwindow.activeStoryPage.AddLineToListBoxWithDelay(receivedData);
                }
                catch (SocketException ex)
                {
                    ClassLibrary.Logger.LogByTemplate(LogEventLevel.Error, ex, "Socket Exception while receiving lore messages");
                    break;
                }
                catch (Exception ex)
                {
                    ClassLibrary.Logger.LogByTemplate(LogEventLevel.Error, ex, "An error occurred while receiving lore messages.");
                    break;
                }
            }
        }
        public async Task<string> ReceiveMessage()
        {
            if (IsConnected)
            {
                try
                {
                    byte[] bytes = new byte[1024];
                    {
                        int bytesRec = await sender.ReceiveAsync(new ArraySegment<byte>(bytes), SocketFlags.None);
                        return Encoding.UTF8.GetString(bytes, 0, bytesRec);
                    }
                }
                catch (SocketException ex)
                {
                    Logger.LogByTemplate(LogEventLevel.Error, ex, "Error While ReceiveMessage");
                    return null;
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
                try
                {
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                    IsConnected = false;
                }
                catch (SocketException ex)
                {
                    Logger.LogByTemplate(LogEventLevel.Error, ex, note: "Error while clicking to disconnet");
                }
            }
        }
    }
}
