using ClassLibrary;
using Microsoft.Win32;
using Serilog.Events;

namespace Client
{
    public class FileHandler
    {
        private static Dictionary<string, string> fileTypes = new()
        {
            { "Image", "Image files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp" },
            { "Media", "Media files (*.mp4;*.avi;*.wmv)|*.mp4;*.avi;*.wmv" }
        };
        public static string OpenFile(string fileType)
        {
            OpenFileDialog ofd = new()
            {
                Multiselect = false,
                Filter = fileTypes[fileType]
            };
            if(ofd.ShowDialog() == true)
            {
                string filename = ofd.FileName;
                string shortFileName = Logger.GetLastFile(filename);
                if (!System.IO.File.Exists(filename))
                {
                    Logger.LogByTemplate(LogEventLevel.Error, note: "Selected file does not exist: " + shortFileName);
                    return null;
                }
                return filename;
            }
            else
            {
                Logger.LogByTemplate(LogEventLevel.Warning, note: "No file selected.");
                return null;
            }
        }
    }
}
