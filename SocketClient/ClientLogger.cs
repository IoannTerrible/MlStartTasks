using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketClient
{
    internal class ClientLogger
    {
        public static void LogByTemplate(LogEventLevel logEventLevel, Exception ?ex = null, string note = "")
        {
            StackTrace stackTrace = new StackTrace(true);
            StringBuilder info = new StringBuilder($"Note: {note}");

            if (ex != null)
            {
                info.AppendLine($"{ex.Message}");
                stackTrace = new StackTrace(ex, true);
            }

            StackFrame? frame = new StackFrame();
            for (int i = 0; i < stackTrace.FrameCount; i++)
            {
                if (stackTrace.GetFrame(i).GetFileLineNumber() != 0) // Поиск нужного фрейма
                {
                    frame = stackTrace.GetFrame(i);
                    break;
                }
            }
            info.AppendLine($"[{logEventLevel}] " +
                $" File: {frame.GetFileName()}," +
                $" Line: {frame.GetFileLineNumber()}," +
                $" Column: {frame.GetFileColumnNumber()}," +
                $" Method: {frame.GetMethod()}");
            Log.Write(logEventLevel, info.ToString());
        }

        public static void CreateLogDirectory(params LogEventLevel[] logEventLevels)
        {
            string logDirectory = Path.Combine("logs", $"{DateTime.Now.Day} ,{DateTime.Now.ToString("MMM")} ,{DateTime.Now.Year}");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
                ClientLogger.LogByTemplate(LogEventLevel.Information, note: "Create logs directory");
            }

            var loggerConfig = new LoggerConfiguration().MinimumLevel.Verbose();
            string logName = string.Empty;

            foreach (var logEventLevel in logEventLevels)
            {
                logName = logEventLevel.ToString().ToLower() + "Log";
                loggerConfig.WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(evt => evt.Level == logEventLevel)
                    .WriteTo.File($@"{logDirectory}\{logName}.txt", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"));
            }

            Log.Logger = loggerConfig.CreateLogger();
        }
    }
}
