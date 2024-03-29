﻿using ClassLibrary;
using Microsoft.VisualBasic;
using Serilog.Events;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerHost
{

    internal class Program
    {
        private static Task? sendLinesTask;
        private static List<User> connectedUsers = new List<User>();
        public static string[]? ContentFromServerConfig { get; set; }
        #region MainMethods
        static async Task Main(string[] args)
        {
            IPEndPoint ipEndPoint = null;
            Socket sListener = null;

            InitializeLogging();
            string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xml");
            ConfigureServerConfig(filePath);

            string[] tempString = { ContentFromServerConfig[0], ContentFromServerConfig[1], ContentFromServerConfig[2] };
            MainFunProgram.GetNumbersFromSendedArrayOfStrings(tempString);

            IPHostEntry ipHost = Dns.GetHostEntry(ContentFromServerConfig[4]);
            IPAddress ipAddr = ipHost.AddressList[0];

            if (int.TryParse(ContentFromServerConfig[3], out int port))
            {
                ipEndPoint = new IPEndPoint(ipAddr, port);
                sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                Console.WriteLine($"Server was started. IP: {ContentFromServerConfig[4]} Port: {port}. Check config for edits.");
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
                User user = new User(handler.RemoteEndPoint.ToString(), port: ipEndPoint.Port);
                connectedUsers.Add(user);
                while (handler.Connected)
                {
                    byte[] bytes = new byte[1024];
                    int bytesRec = await handler.ReceiveAsync(new ArraySegment<byte>(bytes), SocketFlags.None);

                    string data = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                    string reply = await ProcessData(user, data, ipEndPoint, handler);
                    if (reply != "")
                    {
                        byte[] msg = Encoding.UTF8.GetBytes(reply);
                        await handler.SendAsync(new ArraySegment<byte>(msg), SocketFlags.None);

                    }
                    if (data.Contains("<TheEnd>") )
                    {
                        Console.WriteLine($"Client with next login: {user.Login} leave us.");
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
        static async Task<string> ProcessData(User user,string data, IPEndPoint ipEndPoint, Socket handler)
        {
            try
            {
                string[] parts = data.Split(' ');
                string command = parts[0];
                Console.WriteLine(DateAndTime.Now.ToString() + $" Command: {command.Trim().ToUpper()} {user.Login}");
                switch (command.Trim().ToUpper())
                {
                    case "LOG":
                        string login = parts[1];
                        string password = parts[2];
                        if (connectedUsers.Any(u => u.GetLogin() == login))
                        {
                            return "User with this login is already logged in";
                        }

                        if (user.IsLoggedIn)
                        {
                            return "Already logged in";
                        }

                        if (!ClassForAuth.CheckHashAndLog(login, password))
                        {
                            return "Invalid credentials";
                        }
                        else
                        {
                            user.IsLoggedIn = true;
                            user.SetLogin(login);
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
                        if (user != null)
                        {
                            if (user.LoreTask != null && !user.LoreTask.IsCompleted) 
                            {
                                user.CancelLOR();
                                return "";
                            }
                            else
                            {
                                user.ContinueLOR();
                                user.LoreTask = Task.Run(() => SendLinesWithDelay(handler, user.TokenSource.Token));
                                return "";
                            }
                        }
                        else
                        {
                            return "User not found";
                        }
                    case "DIS":
                        Console.WriteLine($"Client with next login: {user.Login} leave us.");
                        connectedUsers.Remove(user);
                        handler.Close();
                        return "";
                        
                    default:
                        return "Incorrect Command";
                }

            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(LogEventLevel.Error, ex, "Error while processing data");
                return $"Error {ex.Message}";
            }
        }

        static async Task SendLinesWithDelay(Socket handler, CancellationToken cancelToken)
        {
            try
            {
                string[] linesCopy = MainFunProgram.Lines.ToArray();

                foreach (string line in linesCopy)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        return;
                    }
                    byte[] msg = Encoding.UTF8.GetBytes(line);
                    await handler.SendAsync(new ArraySegment<byte>(msg), SocketFlags.None);
                    await Task.Delay(TimeSpan.FromSeconds(MainFunProgram.DelayInMilliseconds/1000)); // Задержка перед отправкой следующего элемента
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
            finally
            {
                byte[] endMsg = Encoding.UTF8.GetBytes("S");
                await handler.SendAsync(new ArraySegment<byte>(endMsg), SocketFlags.None);
                #pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
                MainFunProgram.Lines.Clear();
                #pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.
                MainFunProgram.ProcessActions();
            }
        }
        #endregion
        #region HelpersMethods
        static void InitializeLogging()
        {
            Logger.CreateLogDirectory(
                LogEventLevel.Debug,
                LogEventLevel.Information,
                LogEventLevel.Warning,
                LogEventLevel.Error
            );
        }
        static void ConfigureServerConfig(string filePath)
        {
            Logger.LogByTemplate(LogEventLevel.Debug, note: "Checking and configuring file for server");
            Logger.LogByTemplate(LogEventLevel.Information, note: $"Config file path: {filePath}");

            if (!File.Exists(filePath))
            {
                Logger.LogByTemplate(LogEventLevel.Debug, note: "Config file not found, creating with default content");
                ConfigCreator.CreateDefaultConfigFileForServer(filePath);
            }

            ContentFromServerConfig = ConfigReader.ReadConfigFromFile(filePath);
        }
        #endregion
    }
}