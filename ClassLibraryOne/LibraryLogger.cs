using Serilog;
using Serilog.Events;
using System.Diagnostics;
using System.Text;
namespace ClassLibraryOne
{
    public class Logger
    {
        public static void LogByTemplate(LogEventLevel logEventLevel, Exception ex = null, string note = "")
        {
            StackTrace stackTrace = new StackTrace(true);
            StringBuilder info = new StringBuilder($"\nNote: {note}");

            if (ex != null)
            {
                info.AppendLine($"\n{ex.Message}\n");
                stackTrace = new StackTrace(ex, true);
            }

            StackFrame? frame = new StackFrame();
            for (int i = 0; i < stackTrace.FrameCount; i++)
                if (stackTrace.GetFrame(i).GetFileLineNumber() != 0) // Поиск нужного фрейма
                {
                    frame = stackTrace.GetFrame(i);
                    break;
                }

            info.AppendLine($"File: {frame.GetFileName()}\n");
            info.AppendLine($"Line: {frame.GetFileLineNumber()}\n");
            info.AppendLine($"Column: {frame.GetFileColumnNumber()}\n");
            info.AppendLine($"Method: {frame.GetMethod()}\n");

            Log.Write(logEventLevel, info.ToString());

        }
        public static void CreateLogDirectory(params LogEventLevel[] logEventLevels)
        {
            if (!Directory.Exists("logs"))
            {
                Directory.CreateDirectory("logs2");
                Logger.LogByTemplate(LogEventLevel.Information, note: "Create logs directory");
            }
            var loggerConfig = new LoggerConfiguration().MinimumLevel.Verbose();
            string logName = string.Empty;
            foreach (var logEventLevel in logEventLevels)
            {
                logName = logEventLevel.ToString().ToLower() + "Log";
                loggerConfig.WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(evt => evt.Level == logEventLevel)
                .WriteTo.File($@"logs\{logName}.txt"));
            };
            Log.Logger = loggerConfig.CreateLogger();
        }

    }
}
