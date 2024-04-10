using ClassLibrary;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Serilog.Events;
using SocketClient;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Client
{
    internal class VideoController
    {
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

                if (!_videoCapture.IsOpened())
                {
                    return;
                }
                _window = window;
                _videoCapture.Open(filepath);
                SetFirstFrame();
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(LogEventLevel.Error, ex, note: "Error during media file initialization:");
                throw ex;
            }
        }
        #endregion
        #region Attributes
        public MainWindow _window;

        public Image mediaPlayer;
        public Slider mediaSlider;

        private VideoCapture _videoCapture;

        private Mat _frame;
        private BitmapImage bitmapImage;

        private int _currentFrameNumber;
        private int _countFrames;
        private int _fps;

        private List<List<ObjectOnPhoto>> _objectsOnFrame;

        private bool _IsPaused = false;
        #endregion
        #region Methods
        public async void Play()
        {
            _IsPaused = false;
            while (!_IsPaused)
            {
                await SetFrame();
                Cv2.WaitKey(1);
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
                bitmapImage = ImageSourceForImageControl(_frame.ToBitmap());
                _window.activyVideoPage.VideoImage.Source = bitmapImage;
                _window.activyVideoPage.localDrawer.ClearRectangles();
                _window.activyVideoPage.localDrawer.CalculateScale();
                if (_currentFrameNumber - 1 < _objectsOnFrame.Count && _currentFrameNumber > 0)
                {
                    _window.activyVideoPage.localDrawer.DrawBoundingBoxes(_objectsOnFrame[_currentFrameNumber - 1]);
                }
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
        private async Task SetFirstFrame()
        {
            if (_currentFrameNumber < _countFrames)
            {
                _videoCapture.Set(VideoCaptureProperties.PosFrames, _currentFrameNumber);
                _videoCapture.Read(_frame);
                _currentFrameNumber++;
                _window.activyVideoPage.localDrawer.ClearRectangles();
                _window.activyVideoPage.localDrawer.CalculateScale();
                bitmapImage = ImageSourceForImageControl(_frame.ToBitmap());
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
        private async Task SetFrame()
        {
            if (_currentFrameNumber < _countFrames)
            {
                _videoCapture.Set(VideoCaptureProperties.PosFrames, _currentFrameNumber);
                _videoCapture.Read(_frame);
                _currentFrameNumber++;
                _window.activyVideoPage.localDrawer.ClearRectangles();
                _window.activyVideoPage.localDrawer.CalculateScale();
                bitmapImage = ImageSourceForImageControl(_frame.ToBitmap());
                //await MainWindow.apiClient.SendImageAndReceiveJSONAsync(bitmapImage, ConnectionWindow.ConnectionUri);

                _window.activyVideoPage.VideoImage.Source = bitmapImage;
                if (_currentFrameNumber - 1 < _objectsOnFrame.Count)
                {
                    _window.activyVideoPage.localDrawer.DrawBoundingBoxes(_objectsOnFrame[_currentFrameNumber - 1]);
                }
            }
            else
            {
                _currentFrameNumber = 0;
                _videoCapture.Set(VideoCaptureProperties.PosFrames, 0);
                _IsPaused = true;
            }
            mediaSlider.Value = _currentFrameNumber;
        }
        public BitmapImage ImageSourceForImageControl(System.Drawing.Bitmap bitmap)
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
        public async void GetProcessedVideo()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            _objectsOnFrame = await MainWindow.apiClient.GetObjectsOnFrames(_videoCapture, ConnectionWindow.ConnectionUri);
            stopwatch.Stop();
            MessageBox.Show($"Success, {_objectsOnFrame.Count}, times - {stopwatch.ElapsedMilliseconds/1000}s");
        }
        #endregion
    }
}
