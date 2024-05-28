using ClassLibrary;
using Microsoft.VisualBasic;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Serilog.Events;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Client
{
    public class VideoController
    {
        #region Constructor
        public VideoController(string filepath, Image imagePlace, Slider slider, MainWindow window)
        {
            try
            {
                _videoCapture = new VideoCapture(filepath);
                _videoCaptureForProcess = new VideoCapture(filepath);

                _frame = new Mat();
                mediaPlayer = imagePlace;
                mediaSlider = slider;

                currentFrameNumber = 0;
                _countFrames = _videoCapture.FrameCount;
                _fps = (int)_videoCapture.Fps;

                mediaSlider.Value = 0;
                mediaSlider.Minimum = 0;
                mediaSlider.Maximum = _countFrames;

                if (!_videoCapture.IsOpened())
                {
                    return;
                }
                _window = window;
                _videoCapture.Open(filepath);
                shortName = Logger.GetLastFile(filepath);

                double totalSeconds = _countFrames / _fps;
                _minutes = (int)(totalSeconds / 60);
                _seconds = (int)(totalSeconds % 60);

                vtimer = new(_countFrames, _fps);
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
        public VideoTimer vtimer;
        public int currentFrameNumber;

        public Image mediaPlayer;
        public Slider mediaSlider;

        private VideoCapture _videoCapture;
        private VideoCapture _videoCaptureForProcess;
        private VideoWriter _videoWriter;

        private Mat _frame;
        private BitmapImage bitmapImage;

        private int _countFrames;
        private int _fps;
        private int _seconds;
        private int _minutes;

        public string shortName;

        public List<List<ObjectOnPhoto>> ObjectsOnFrame;
        public bool IsProcessed = false;

        public bool IsPaused = false;
        #endregion
        #region Methods
        public async void Play()
        {
            IsPaused = false;
            while (!IsPaused)
            {
                await SetFrame();
                Cv2.WaitKey(500 / _fps);
            }
        }
        public async void Stop()
        {
            currentFrameNumber = 0;
            _videoCapture.Set(VideoCaptureProperties.PosFrames, 0);
            await SetFrame();
            IsPaused = true;
        }
        public void Pause()
        {
            IsPaused = !IsPaused;
            if (!IsPaused) Play();
        }
        public async void Rewind()
        {
            IsPaused = true;
            if (_videoCapture.Set(VideoCaptureProperties.PosFrames, currentFrameNumber - 1) && currentFrameNumber > 0)
            {
                _videoCapture.Read(_frame);
                if (ObjectsOnFrame != null)
                {
                    _window.activyVideoPage.localDrawer.ClearRectangles();
                    _window.activyVideoPage.localDrawer.CalculateScale();
                    if (currentFrameNumber - 1 < ObjectsOnFrame.Count && currentFrameNumber > 0)
                    {
                        bitmapImage = ImageConverter.ImageSourceForImageControl
                            (
                            (_window.activyVideoPage.localDrawer.DrawBoundingBoxes(ObjectsOnFrame[currentFrameNumber - 1], _frame).ToBitmap())
                            );
                    }
                }
                else
                {
                    bitmapImage = ImageConverter.ImageSourceForImageControl(_frame.ToBitmap());

                }
                _window.activyVideoPage.VideoImage.Source = bitmapImage;
                currentFrameNumber--;
                mediaSlider.Value = currentFrameNumber;
            }
            else MessageBox.Show("There is no road", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        public async void NextFrame()
        {
            IsPaused = true;
            await SetFrame();
        }
        public async void ShowInfo()
        {
            MessageBox.Show($@"Frames = {_countFrames},
                            Current Frame = {currentFrameNumber},
                            OriginalFrames Per Second = {_fps},
                            Time {_minutes}:{_seconds:D2}");
        }
        public async void GetSliderValue(double value)
        {
            mediaSlider.Value = value;
            currentFrameNumber = (int)mediaSlider.Value;
        }
        public async Task SetFirstFrame()
        {
            if (currentFrameNumber < _countFrames && currentFrameNumber >= 0)
            {
                _videoCapture.Set(VideoCaptureProperties.PosFrames, currentFrameNumber);
                _videoCapture.Read(_frame);
                currentFrameNumber++;
                _window.activyVideoPage.localDrawer.ClearRectangles();
                _window.activyVideoPage.localDrawer.CalculateScale();
                bitmapImage = ImageConverter.ImageSourceForImageControl(_frame.ToBitmap());
                await MainWindow.apiClient.SendImageAndReceiveJSONAsync(bitmapImage, ConnectionWindow.ConnectionUri);
            }
            else
            {
                currentFrameNumber = 0;
                _videoCapture.Set(VideoCaptureProperties.PosFrames, 0);
                IsPaused = true;
            }
            mediaSlider.Value = currentFrameNumber;
        }
        private async Task SetFrame()
        {
            if (currentFrameNumber < _countFrames && currentFrameNumber >= 0)
            {
                _videoCapture.Set(VideoCaptureProperties.PosFrames, currentFrameNumber);
                _videoCapture.Read(_frame);
                currentFrameNumber++;
                vtimer.UpdateCurrentFrame(currentFrameNumber);
                _window.activyVideoPage.TimerBox.Text = vtimer.GetCurrentTime();
                _window.activyVideoPage.localDrawer.ClearRectangles();

                if (ObjectsOnFrame != null)
                {
                    if (currentFrameNumber - 1 < ObjectsOnFrame.Count)
                    {
                        _window.activyVideoPage.localDrawer.CalculateScale();
                        bitmapImage = ImageConverter.ImageSourceForImageControl((_window.activyVideoPage.localDrawer.DrawBoundingBoxes(ObjectsOnFrame[currentFrameNumber - 1], _frame).ToBitmap()));
                    }
                }
                else
                {
                    bitmapImage = ImageConverter.ImageSourceForImageControl(_frame.ToBitmap());
                }
                _window.activyVideoPage.VideoImage.Source = bitmapImage;
            }
            else
            {
                currentFrameNumber = 0;
                _videoCapture.Set(VideoCaptureProperties.PosFrames, 0);
                IsPaused = true;
            }
            mediaSlider.Value = currentFrameNumber;
        }

        public async void GetProcessedVideo()
        {
            try
            {
                if (ObjectsOnFrame == null)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    if (App.ContentFromConfig["ProcessInRealTime"] == "false")
                    {
                        ObjectsOnFrame = await MainWindow.apiClient.GetObjectsOnFrames(_videoCaptureForProcess, ConnectionWindow.ConnectionUri);
                    }
                    else
                    {
                        await MainWindow.apiClient.GetProcessedInRealTimeVideo(_videoCaptureForProcess, this, ConnectionWindow.ConnectionUri);
                    }
                    stopwatch.Stop();
                    MessageBox.Show($"Success, {ObjectsOnFrame.Count}, times - {stopwatch.ElapsedMilliseconds / 1000}s");
                    Logger.LogByTemplate(LogEventLevel.Information, note: $"Video processed, frames count - {_countFrames}, frames processed - {ObjectsOnFrame.Count}");
                }
                else
                {
                    if (ObjectsOnFrame.Count != _countFrames)
                    {
                        ObjectsOnFrame = null;
                        GetProcessedVideo();
                    }
                    else
                    {
                        Logger.LogByTemplate(LogEventLevel.Warning, note: "An attempt to re-process the video.");
                        MessageBox.Show($"Video processed. frames - {ObjectsOnFrame.Count}");
                    }
                }
                IsProcessed = true;
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(LogEventLevel.Error, ex, note: "Video processing error.");
            }
        }

        public void SaveFullVideo()
        {
            try
            {
                if (ObjectsOnFrame is null) return;
                string path = FileHandler.SaveVideoFile(shortName);
                _videoWriter = new(path, FourCC.FromString(_videoCapture.FourCC), _videoCapture.Fps, new OpenCvSharp.Size(_videoCapture.FrameWidth, _videoCapture.FrameHeight));
                Mat matFrame = new();
                for (int i = 0; i < _videoCapture.FrameCount; i++)
                {
                    _videoCaptureForProcess.Set(VideoCaptureProperties.PosFrames, i);
                    _videoCaptureForProcess.Read(matFrame);
                    if (i < ObjectsOnFrame.Count)
                    {
                        _videoWriter.Write(_window.activyVideoPage.localDrawer.DrawBoundingBoxes(ObjectsOnFrame[i], matFrame));
                    }
                    else _videoWriter.Write(matFrame);
                }
                _videoWriter.Release();
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(LogEventLevel.Error, ex, "error save file");
                MessageBox.Show("error save");
            }
        }
        #endregion
    }
}
