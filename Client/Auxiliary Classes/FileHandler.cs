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
        public static string SaveVideoFile(string filename, int num, int objectID)
        {
            SaveFileDialog saveFileDialog = new()
            {
                Filter = fileTypes[FileTypes.Media],
                DefaultExt = ".avi",
                FileName = filename.Split('.')[0] + $"num{num}objID{objectID}"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                return saveFileDialog.FileName;
            }
            else
            {
                return null;
            }
        }
        public static string SaveVideoFile(string filename)
        {
            SaveFileDialog saveFileDialog = new()
            {
                Filter = fileTypes[FileTypes.Media],
                DefaultExt = ".avi",
                FileName = filename.Split('.')[0] + $"Trim"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                return saveFileDialog.FileName;
            }
            else
            {
                return null;
            }
        }

    }
}
