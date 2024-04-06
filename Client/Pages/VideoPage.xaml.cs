using ClassLibrary;
using Microsoft.Win32;
using Serilog.Events;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SocketClient
{
    /// <summary>
    /// Логика взаимодействия для VideoPage.xaml
    /// </summary>
    public partial class VideoPage : Page
    {
        public VideoPage()
        {
            InitializeComponent();
        }

        private void MediaPlayButton_Click(object sender, RoutedEventArgs e)
        {
            mediaElement1.Play();
        }

        private void MediaStopButton_Click(object sender, RoutedEventArgs e)
        {
            mediaElement1.Stop();
        }

        private void UploadMediaButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Multiselect = false,
                Filter = "Media files (*.mp4;*.avi)|*.mp4;*.avi"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filename = openFileDialog.FileName;
                string shortFileName = Logger.GetLastFile(filename);
                if (!System.IO.File.Exists(filename))
                {
                    Logger.LogByTemplate(LogEventLevel.Error, note: "Selected file does not exist: " + shortFileName);
                    return;
                }
                mediaElement1.Source = new Uri(filename);
                Logger.LogByTemplate(LogEventLevel.Information, note: $"media file opened from {filename}");
                if(mediaElement1.Source != null)
                {
                    mediaElement1.Play();
                }
            }
            else
            {
                Logger.LogByTemplate(LogEventLevel.Warning, note: "No file selected.");
            }
        }
    }
}
