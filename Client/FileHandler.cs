using ClassLibrary;
using Microsoft.Win32;
using Serilog.Events;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace Client
{
    public enum FileTypes
    {
        Image,
        Media
    }
    public class FileHandler
    {
        public static string LastKeyFrameName { get; set; }

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
        public static void SaveBitmapImageToFile(Bitmap bitmapImage)
        {
            string directoryPath = Path.Combine(App.PathToDirectory, "KeyFrames");
            Directory.CreateDirectory(directoryPath);
            LastKeyFrameName = $"Image_{DateTime.Now:yyyyMMddHHmmssfff}.png";
            string filePath = Path.Combine(directoryPath, LastKeyFrameName);
            ImageFormat format = ImageFormat.Png;
            bitmapImage.Save(filePath, format);
        }

    }
}
