using ClassLibrary;
using Client;
using Serilog.Events;
using System.Windows;
using System.Windows.Controls;
namespace SocketClient
{
    /// <summary>
    /// Логика взаимодействия для VideoPage.xaml
    /// </summary>
    public partial class VideoPage : Page
    {
        private readonly Canvas rectangleContainer = new();
        public readonly Drawer localDrawer;

        public VideoPage(MainWindow window)
        {
            InitializeComponent();
            _window = window;
            Canvas canvas = FindName("canvas2") as Canvas;
            canvas.Children.Add(rectangleContainer);
            localDrawer = new Drawer(rectangleContainer, VideoImage);
        }

        private string filepath;
        VideoController videoController;
        MainWindow _window;
        private double originalWidth;
        private double originalHeight;
        private double scaleX;
        private double scaleY;

        private void MediaPlayButton_Click(object sender, RoutedEventArgs e)
        {
            videoController.Play();
        }

        private void MediaPauseButton_Click(object sender, RoutedEventArgs e)
        {
            videoController.Pause();
        }

        private void MediaStopButton_Click(object sender, RoutedEventArgs e)
        {
            videoController.Stop();
        }

        private void RewindButton_Click(object sender, RoutedEventArgs e)
        {
            videoController.Rewind();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            videoController.NextFrame();
        }
        private void ShowInfoButton_Click(object sender, RoutedEventArgs e)
        {
            videoController.ShowInfo();
        }
        private void MediaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider)sender).SelectionEnd = e.NewValue;
            if (videoController != null)
            {
                videoController.GetSliderValue(e.NewValue);
            }
        }
        private void UploadMediaButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                filepath = FileHandler.OpenFile("Media");
                if (filepath != null)
                {
                    videoController = new(filepath, VideoImage, MediaSlider, _window);
                    MediaSlider.Value = 0;
                }
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(LogEventLevel.Error, ex, note: "Media file opening error.");
                MessageBox.Show($"Media file opening error: {ex.Message}");
            }
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            //ListBoxForResponce.Items.Add(SqlCore.ReturnLogEventAsString(MainWindow.connectionString));
            videoController.GetProcessedVideo();
        }

        private async void HealthCheckButton_Click(object sender, RoutedEventArgs e)
        {
            if (await MainWindow.apiClient.CheckHealthAsync($"{ConnectionWindow.ConnectionUri}health"))
            {
                MessageBox.Show("Yes");
            }
        }
        private void VideoBox_SourceUpdated(object sender, RoutedEventArgs e)
        {
            localDrawer.CalculateScale();
            localDrawer.ClearRectangles();
            Logger.LogByTemplate(LogEventLevel.Information, note: $"Image uploaded.");
        }
    }
}
