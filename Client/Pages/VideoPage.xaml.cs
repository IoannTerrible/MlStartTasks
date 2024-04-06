using ClassLibrary;
using Client;
using Serilog.Events;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SocketClient
{
    /// <summary>
    /// Логика взаимодействия для VideoPage.xaml
    /// </summary>
    public partial class VideoPage : Page
    {
        private DispatcherTimer timer = new();
        public VideoPage()
        {
            InitializeComponent();
            timer.Interval = TimeSpan.FromSeconds(0.1);
            timer.Tick += timer_tick;
        }

        private void timer_tick(object sender, EventArgs e)
        {
            time.Text = mediaElement1.Position.ToString(@"mm\:ss");
        }

        private void MediaPlayButton_Click(object sender, RoutedEventArgs e)
        {
            mediaElement1.Play();
            timer.Start();
        }

        private void MediaPauseButton_Click(object sender, RoutedEventArgs e)
        {
            mediaElement1.Pause();
            timer.Stop();
        }

        private void MediaStopButton_Click(object sender, RoutedEventArgs e)
        {
            mediaElement1.Stop();
            timer.Stop();
        }
        private void slider2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaElement1.Pause();
            mediaElement1.Play();
            timer.Start();
        }
        private void mediaElement1_MediaOpened(object sender, RoutedEventArgs e)
        {

        }

        private void UploadMediaButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string? filename = FileHandler.OpenFile("Media");

                if (filename != null)
                {
                    mediaElement1.Source = new Uri(filename);
                    Logger.LogByTemplate(LogEventLevel.Information, note: $"Media file opened from {filename}");
                    MediaPlayButton_Click(sender, e);
                }
                mediaElement1.Source = new Uri(filename);
                Logger.LogByTemplate(LogEventLevel.Information, note: $"media file opened from {filename}");
                if (mediaElement1.Source != null)
                {
                    mediaElement1.Play();
                }
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(LogEventLevel.Error, ex, note: "Media file openning error.");
                MessageBox.Show($"An unexpected error occurred: {ex.Message}");
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            ListBoxForResponce.Items.Add(SqlCore.ReturnLogEventAsString(MainWindow.connectionString));
        }

        private void PrevFrameButton_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan currentPosition = mediaElement1.Position;
            double framesPerSecond = 1;
            TimeSpan prevFramePosition = currentPosition.Subtract(TimeSpan.FromSeconds(1 / framesPerSecond));

            mediaElement1.Pause();
            mediaElement1.Position = prevFramePosition;
            mediaElement1.Play();
        }
        private void NextFrameButton_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan currentPosition = mediaElement1.Position;
            double framesPerSecond = 1;
            TimeSpan nextFramePosition = currentPosition.Add(TimeSpan.FromSeconds(1 / framesPerSecond));

            mediaElement1.Pause(); 
            mediaElement1.Position = nextFramePosition;
            mediaElement1.Play();
        }

        //public double GetFrameRate(string filePath)
        //{

        //}
    }
}
