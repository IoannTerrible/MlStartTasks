using ClassLibrary;
using Client;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Serilog.Events;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
namespace SocketClient
{
    /// <summary>
    /// Логика взаимодействия для VideoPage.xaml
    /// </summary>
    public partial class VideoPage : Page
    {
        MainWindow _window;
        public VideoPage(MainWindow window)
        {
            InitializeComponent();
            _window = window;
        }

        private VideoCapture _videoCapture;

        private Mat _frame;

        private string filepath;
        private int _currentFrameNumber;
        private int _countFrames;

        private bool _IsPaused = false;
        private bool _IsStopped = true;

        private readonly Canvas rectangleContainer = new();

        private double originalWidth;
        private double originalHeight;
        private double scaleX;
        private double scaleY;

        private void MediaPlayButton_Click(object sender, RoutedEventArgs e)
        {
            _IsPaused = false;
            while (!_IsPaused && !_IsStopped)
            {
                SetFrame();
                if (Cv2.WaitKey(1) == 113) // Q
                    break;
            }
        }

        private void MediaPauseButton_Click(object sender, RoutedEventArgs e)
        {
            _IsPaused = !_IsPaused;
        }

        private void MediaStopButton_Click(object sender, RoutedEventArgs e)
        {
            _IsPaused = true;
            _currentFrameNumber = 0;
            _videoCapture.Set(VideoCaptureProperties.PosFrames, 0);
            _IsPaused = true;
        }

        private void RewindButton_Click(object sender, RoutedEventArgs e)
        {
            if (_videoCapture.Set(VideoCaptureProperties.PosFrames, _currentFrameNumber - 1))
            {
                _videoCapture.Read(_frame);
                VideoImage.Source = imageSourceForImageControl(_frame.ToBitmap());
                _currentFrameNumber--;
            }
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            SetFrame();
        }

        private void UploadMediaButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                filepath = FileHandler.OpenFile("Media");


                _videoCapture = new VideoCapture(filepath);
                _frame = new Mat();

                _currentFrameNumber = 0;
                _countFrames = _videoCapture.FrameCount;
                
                _IsStopped = false;

                if (!_videoCapture.IsOpened())
                {
                    return;
                }

                _videoCapture.Open(filepath);
                SetFrame();
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

        private void SetFrame()
        {
            if(_currentFrameNumber  < _countFrames)
            {
                _videoCapture.Read(_frame);
                _currentFrameNumber++;
                VideoImage.Source = imageSourceForImageControl(_frame.ToBitmap());
            }
            else
            {
                _currentFrameNumber = 0;
                _videoCapture.Set(VideoCaptureProperties.PosFrames, 0);
                _IsPaused = true;
            }
        }

        private BitmapImage imageSourceForImageControl(System.Drawing.Bitmap bitmap)
        {
            {
                using (MemoryStream memory = new())
                {
                    bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                    memory.Position = 0;
                    BitmapImage bitmapimage = new();
                    bitmapimage.BeginInit();
                    bitmapimage.StreamSource = memory;
                    bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapimage.EndInit();
                    MainWindow.apiClient.SendImageAndReceiveJSONAsync(bitmapimage, ConnectionWindow.ConnectionUri);
                    return bitmapimage;
                }
            }
        }
        public void DrawBoundingBoxes(List<ObjectOnPhoto> aircraftObjects)
        {
            foreach (var obj in aircraftObjects)
            {
                double xtl = obj.XTopLeftCorner;
                double ytl = obj.YTopLeftCorner;
                double xbr = obj.XBottonRigtCorner;
                double ybr = obj.YBottonRigtCorner;

                DrawBoundingBox(xtl, ytl, xbr, ybr);
            }
            Logger.LogByTemplate(LogEventLevel.Information, note: $"{aircraftObjects.Count} borders of objects have been drawn.");
        }
        private void DrawBoundingBox(double xTopLeft, double yTopLeft, double xBottomRight, double yBottomRight)
        {
            CalculateScale();
            Rectangle boundingBox = new Rectangle();

            double scaledXTopLeft = xTopLeft * scaleX;
            double scaledYTopLeft = yTopLeft * scaleY;
            double scaledWidth = (xBottomRight - xTopLeft) * scaleX;
            double scaledHeight = (yBottomRight - yTopLeft) * scaleY;

            boundingBox.Width = scaledWidth;
            boundingBox.Height = scaledHeight;
            Canvas.SetLeft(boundingBox, scaledXTopLeft);
            Canvas.SetTop(boundingBox, scaledYTopLeft);

            boundingBox.Stroke = Brushes.Red;
            boundingBox.StrokeThickness = 2;
            boundingBox.Fill = Brushes.Transparent;

            rectangleContainer.Children.Add(boundingBox);
        }
        private void ClearRectangles()
        {
            rectangleContainer.Children.Clear();
        }
        private void VideoBox_SourceUpdated(object sender, RoutedEventArgs e)
        {
            CalculateScale();
            ClearRectangles();
            Logger.LogByTemplate(LogEventLevel.Information, note: $"Image uploaded.");
        }
        private void CalculateScale()

        {
            if (VideoImage.Source is BitmapSource)
            {
                BitmapSource bitmapSource = (BitmapSource)VideoImage.Source;
                double currentWidth = VideoImage.ActualWidth;
                double currentHeight = VideoImage.ActualHeight;

                originalWidth = bitmapSource.Width;
                originalHeight = bitmapSource.Height;

                scaleX = currentWidth / originalWidth;
                scaleY = currentHeight / originalHeight;
            }
            Logger.LogByTemplate(LogEventLevel.Debug, note: "The image scale is calculated");
        }
    }
}
