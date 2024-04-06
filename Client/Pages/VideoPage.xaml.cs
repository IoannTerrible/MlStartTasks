using ClassLibrary;
using Client;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Serilog.Events;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

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
        
        private VideoCapture _videoCapture;

        private Mat _frame;

        private string filepath;
        private int _currentFrameNumber;
        private int _countFrames;

        private bool _IsPaused = false;
        private bool _IsStopped = true;


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

        private BitmapImage imageSourceForImageControl(Bitmap bitmap)
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

                    return bitmapimage;
                }
            }
        }

    }
}
