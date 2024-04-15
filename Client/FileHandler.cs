using ClassLibrary;
using Microsoft.Win32;
using Serilog.Events;

namespace Client
{
    public enum FileTypes
    {
        Image,
        Media
    }
    public class FileHandler
    {
        private static Dictionary<FileTypes, string> fileTypes = new()
        {
            { FileTypes.Image, "Image files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp" },
            { FileTypes.Media, "Media files (*.mp4;*.avi;*.wmv)|*.mp4;*.avi;*.wmv" }
        };
        public static string[] OpenFile(FileTypes fileType)
        {
            OpenFileDialog ofd = new()
            {
                Multiselect = true,
                Filter = fileTypes[fileType]
            };
            if(ofd.ShowDialog() == true)
            {
                string[] filenames = ofd.FileNames;
                return filenames;
            }
            else
            {
                Logger.LogByTemplate(LogEventLevel.Warning, note: "No file selected.");
                return null;
            }
        }
    }
}
