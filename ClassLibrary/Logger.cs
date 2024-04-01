using Serilog;
using Serilog.Events;
using System.Diagnostics;
using System.Text;

namespace ClassLibrary
{
    public class Logger
    {
        public static void LogByTemplate(LogEventLevel logEventLevel, Exception? ex = null, string note = "")
        {
            StackTrace stackTrace = new StackTrace(true);
            StringBuilder info = new StringBuilder($"Note: {note}");
            if (logEventLevel == LogEventLevel.Debug)
            {
                var (shortCommitId, commitVersion) = GetCommitInfo();
                if (!string.IsNullOrEmpty(shortCommitId) && !string.IsNullOrEmpty(commitVersion))
                {
                    info.Append($" CommitID: {shortCommitId}");
                    info.Append($" AppVersion: {commitVersion}");
                }
            }
            if (ex != null)
            {
                info.Append($"{ex.Message} ");
                stackTrace = new StackTrace(ex, true);


                StackFrame? frame = new StackFrame();
                for (int i = 0; i < stackTrace.FrameCount; i++)
                {
                    if (stackTrace.GetFrame(i).GetFileLineNumber() != 0)
                    {
                        frame = stackTrace.GetFrame(i);
                        break;
                    }
                }
                string fileName = GetLastFile(frame.GetFileName());
                info.AppendLine(
                    $" File: {fileName}," +
                    $" Line: {frame.GetFileLineNumber()}," +
                    $" Column: {frame.GetFileColumnNumber()}," +
                    $" Method: {frame.GetMethod()}");
            }
            else info.AppendLine("");
            Log.Write(logEventLevel, info.ToString());
        }

        public static void CreateLogDirectory(params LogEventLevel[] logEventLevels)
        {
            string logDirectory = Path.Combine("logs", $"{DateTime.Now:yyyy-MM-dd}");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
                LogByTemplate(LogEventLevel.Information, note: "Create logs directory");
            }

            var loggerConfig = new LoggerConfiguration().MinimumLevel.Verbose();
            string logName = string.Empty;

            foreach (var logEventLevel in logEventLevels)
            {
                logName = logEventLevel.ToString().ToLower() + "Log";
                loggerConfig.WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(evt => evt.Level == logEventLevel)
                    .WriteTo.File($@"{logDirectory}\{logName}.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message:lj}"));
            }

            Log.Logger = loggerConfig.CreateLogger();
        }
        public static string GetLastFile(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    throw new ArgumentException("File path cannot be null or empty.");
                }
                string[] pathParts = filePath.Split('\\');
                return pathParts[pathParts.Length - 1];
            }
            catch
            {
                return null;
            }   
        }
        private static (string shortCommitId, string commitVersion) GetCommitInfo()
        {
            try
            {
                Process process = new();
                process.StartInfo.FileName = "git";
                process.StartInfo.Arguments = "log --format=\"%h|%s\" -n 1 HEAD"; 
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                string[] parts = output.Trim().Split('|');
                if (parts.Length == 2)
                {
                    string commitVersion = parts[1].Split(' ')[0];
                    return (parts[0], commitVersion);
                }
                else
                {
                    throw new Exception("Unexpected output format from git log.");
                }
            }
            catch (Exception ex)
            {
                Log.Write(LogEventLevel.Error, ex.ToString());
                return (null, null);
            }
        }
    }
}
