using ClassLibrary;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Serilog.Events;
using SocketClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Client
{
    internal class VideoController
    {
        public MainWindow _window;
        #region Constructor
        public VideoController(string filepath, Image imagePlace, Slider slider, MainWindow window)
        {
            try
            {
                _videoCapture = new VideoCapture(filepath);
                _frame = new Mat();
                mediaPlayer = imagePlace;
                mediaSlider = slider;

                _currentFrameNumber = 0;
                _countFrames = _videoCapture.FrameCount;
                _fps = (int)(1000 / _videoCapture.Fps);

                mediaSlider.Value = 0;
                mediaSlider.Minimum = 0;
                mediaSlider.Maximum = _countFrames;

                _IsStopped = false;

                if (!_videoCapture.IsOpened())
                {
                    return;
                }
                _window = window;
                _videoCapture.Open(filepath);
                SetFrame();
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(LogEventLevel.Error, ex, note: "Error during media file initialization:");
                throw ex;
            }
        }
        #endregion

        #region Attributes
        public Image mediaPlayer;
        public Slider mediaSlider;

        private VideoCapture _videoCapture;

        private Mat _frame;
        private BitmapImage bitmapImage;

        private int _currentFrameNumber;
        private int _countFrames;
        private int _fps;

        private bool _IsPaused = false;
        private bool _IsStopped = true;
        #endregion
        #region Methods
        public async void Play()
        {
            _IsPaused = false;
            while (!_IsPaused && !_IsStopped)
            {
                await SetFrame();
                Cv2.WaitKey(_fps);
            }
        }
        public async void Stop()
        {
            _currentFrameNumber = 0;
            _videoCapture.Set(VideoCaptureProperties.PosFrames, 0);
            await SetFrame();
            _IsPaused = true;
        }
        public void Pause()
        {
            _IsPaused = !_IsPaused;
            if (!_IsPaused) Play();
        }
        public async void Rewind()
        {
            _IsPaused = true;
            if (_videoCapture.Set(VideoCaptureProperties.PosFrames, _currentFrameNumber - 1))
            {
                _videoCapture.Read(_frame);
                bitmapImage = imageSourceForImageControl(_frame.ToBitmap());
                await MainWindow.apiClient.SendImageAndReceiveJSONAsync(bitmapImage, ConnectionWindow.ConnectionUri);
                _currentFrameNumber--;
                mediaSlider.Value = _currentFrameNumber;
            }
        }
        public async void NextFrame()
        {
            _IsPaused = true;
            await SetFrame();
        }
        public async void ShowInfo()
        {
            MessageBox.Show($@"Frames = {_countFrames},
                            Current Frame = {_currentFrameNumber},
                            Frames Per Second = {_fps}");
        }
        public async void GetSliderValue(double value)
        {
            mediaSlider.Value = value;
            _currentFrameNumber = (int)mediaSlider.Value;
        }
        private async Task SetFrame()
        {
            if (_currentFrameNumber < _countFrames)
            {
                _videoCapture.Set(VideoCaptureProperties.PosFrames, _currentFrameNumber);
                _videoCapture.Read(_frame);
                _currentFrameNumber++;
                _window.activyVideoPage.ClearRectangles();
                bitmapImage = imageSourceForImageControl(_frame.ToBitmap());
                await MainWindow.apiClient.SendImageAndReceiveJSONAsync(bitmapImage, ConnectionWindow.ConnectionUri);
            }
            else
            {
                _currentFrameNumber = 0;
                _videoCapture.Set(VideoCaptureProperties.PosFrames, 0);
                _IsPaused = true;
            }
            mediaSlider.Value = _currentFrameNumber;
        }
        public BitmapImage imageSourceForImageControl(System.Drawing.Bitmap bitmap)
        {
            {
                using MemoryStream memory = new();
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
        #endregion
    }
}
