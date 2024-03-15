using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using Microsoft.VisualBasic;
using ClassLibrary;
using Serilog.Events;
using Serilog;
using System.Threading;

namespace ServerHost
{

    internal class Program
    {
        private static CancellationTokenSource loreCancelToken = new CancellationTokenSource();
        private static Task sendLinesTask;

        public static string[] ContentFromServerConfig { get; set; }
        static async Task Main(string[] args)
        {
            IPEndPoint ipEndPoint = null;
            Socket sListener = null;

            Logger.CreateLogDirectory(
                LogEventLevel.Debug,
                LogEventLevel.Information,
                LogEventLevel.Warning,
                LogEventLevel.Error
            );

            
            
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = System.IO.Path.Combine(currentDirectory, "config.xml");

            Logger.LogByTemplate(LogEventLevel.Debug, note: "Checking and configuring file for server");
            Logger.LogByTemplate(LogEventLevel.Information, note: $"Config file path: {filePath}");

            if (!File.Exists(filePath))
            {
                Logger.LogByTemplate(LogEventLevel.Debug, note: "Config file not found, creating with default content ");
                ConfigCreator.CreateDefaultConfigFileForServer(filePath);
            }

            ContentFromServerConfig = ConfigReader.ReadConfigFromFile(filePath);
            string[] tempString = { ContentFromServerConfig[0], ContentFromServerConfig[1], ContentFromServerConfig[2] };
            MainFunProgram.GetNumbersFromSendedArrayOfStrings(tempString);
            IPHostEntry ipHost = Dns.GetHostEntry(ContentFromServerConfig[4]);
            IPAddress ipAddr = ipHost.AddressList[0];

            
            if (int.TryParse(ContentFromServerConfig[3], out int port))
            {
                ipEndPoint = new IPEndPoint(ipAddr, port);
                sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                
                Console.WriteLine($"Server was started. ip:{ContentFromServerConfig[4]} port:{port} Check config for edit");
            }
            else
            {
                Logger.LogByTemplate(LogEventLevel.Error, note: "Unable to parse port number from configuration");
                Console.WriteLine("Error: Unable to parse port number from configuration.");
            }
            MainFunProgram.CoreMain();
            try
            {
                sListener.Bind(ipEndPoint);
                sListener.Listen(10);

                while (true)
                {
                    Socket handler = await sListener.AcceptAsync();
                    _ = Task.Run(async () =>
                    {
                        await HandleClient(handler, ipEndPoint);
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.ReadLine();
            }
        }

        static async Task HandleClient(Socket handler, IPEndPoint ipEndPoint)
        {
            try
            {
                while (true)
                {
                    byte[] bytes = new byte[1024];
                    int bytesRec = await handler.ReceiveAsync(new ArraySegment<byte>(bytes), SocketFlags.None);

                    string data = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                    string reply = await ProcessData(data, ipEndPoint, handler);
                    byte[] msg = Encoding.UTF8.GetBytes(reply);
                    await handler.SendAsync(new ArraySegment<byte>(msg), SocketFlags.None);

                    if (data.Contains("<TheEnd>"))
                    {
                        Console.WriteLine("Клиент завершил соединение.");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
        }
        static async Task<string> ProcessData(string data, IPEndPoint ipEndPoint, Socket handler)
        {
            try
            {
                string[] parts = data.Split(' ');
                string command = parts[0];
                Console.WriteLine("Command: " + command.Trim().ToUpper());
                switch (command.Trim().ToUpper())
                {
                    case "LOG":
                        string login = parts[1];
                        string password = parts[2];
                        //Console.WriteLine($"ConvertedData {parts[0]} {parts[1]} {parts[2]}");
                        if (!ClassForAuth.CheckHashAndLog(login, password))
                        {
                            return "Invalid credentials";
                        }
                        else
                        {
                            return "You have successfully logged in";
                        }
                    case "REG":
                        string loginForReg = parts[1];
                        string passwordForReg = parts[2];
                        return ClassForAuth.RegistrationIn(loginForReg, passwordForReg);
                    case "CON":
                        Console.WriteLine($"Client IP: {handler.RemoteEndPoint}, Port: {ipEndPoint.Port}");
                        return $"You are connected to IP: {ipEndPoint.Address}, Port: {ipEndPoint.Port}";
                    case "LOR":
                        if (sendLinesTask != null && !sendLinesTask.IsCompleted)
                        {
                            CancelOperation();
                            return "Sending operation cancelled";
                        }
                        else
                        {
                            loreCancelToken = new CancellationTokenSource();
                            sendLinesTask = SendLinesWithDelay(handler, loreCancelToken.Token);
                            return "";
                        }

                    default:
                        return "Incorrect Command";
                }

            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(LogEventLevel.Error, ex, "Error while processing data");
                return "Error";
            }
        }
        public static void CancelOperation()
        {
            loreCancelToken.Cancel();
        }
        static async Task SendLinesWithDelay(Socket handler, CancellationToken cancelToken)
        {
            try
            {
                foreach (string line in MainFunProgram.Lines)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return;
                    }
                    byte[] msg = Encoding.UTF8.GetBytes(line);
                    await handler.SendAsync(new ArraySegment<byte>(msg), SocketFlags.None);
                    await Task.Delay(TimeSpan.FromSeconds(MainFunProgram.DelayInSeconds)); // Задержка перед отправкой следующего элемента
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
            finally
            {
                byte[] endMsg = Encoding.UTF8.GetBytes("<EndOfTransmission>");
                await handler.SendAsync(new ArraySegment<byte>(endMsg), SocketFlags.None);
#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
                MainFunProgram.Lines.Clear();
#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.
                MainFunProgram.ProcessActions();
            }
        }
    }
}