using ClassLibrary;
using Microsoft.VisualBasic;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Serilog.Events;
using System.Diagnostics;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Client
{
    public class VideoController : IDisposable
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

        public ObservableCollection<LogEntry> logEntries { get; } = new ObservableCollection<LogEntry>();
        public ObservableCollection<LogEntry> filteredLogEntries = new();
        public List<int[]> valuesForSave = [];
        public List<List<ObjectOnPhoto>> ObjectsOnFrame;
        public bool IsProcessed = false;
        public bool IsProcessing = false;
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
                            (_window.activyVideoPage.localDrawer.DrawBoundingBoxes(ObjectsOnFrame[currentFrameNumber - 1], _frame).ToBitmap());
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
                            Time {_minutes}:{_seconds:D2},
                            FourCC = {_videoCapture.FourCC}");

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
                    else
                    {
                        bitmapImage = ImageConverter.ImageSourceForImageControl(_frame.ToBitmap());
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
            if (_window.activyVideoPage.saveWindow != null)
            {
                _window.activyVideoPage.saveWindow.VideoSource_Update();
                _window.activyVideoPage.saveWindow.TimerBoxForSave.Text = vtimer.GetCurrentTime();
            }
        }
        public async void SetFrame(int frameNumber)
        {
            IsPaused = true;
            if (frameNumber >= 0 && frameNumber < _countFrames)
            {
                currentFrameNumber = frameNumber;
                _videoCapture.Set(VideoCaptureProperties.PosFrames, currentFrameNumber);
                _videoCapture.Read(_frame);

                vtimer.UpdateCurrentFrame(currentFrameNumber);
                _window.activyVideoPage.TimerBox.Text = vtimer.GetCurrentTime();
                _window.activyVideoPage.localDrawer.ClearRectangles();

                if (ObjectsOnFrame != null && currentFrameNumber < ObjectsOnFrame.Count)
                {
                    _window.activyVideoPage.localDrawer.CalculateScale();
                    bitmapImage = ImageConverter.ImageSourceForImageControl(
                        _window.activyVideoPage.localDrawer.DrawBoundingBoxes(ObjectsOnFrame[currentFrameNumber], _frame).ToBitmap()
                    );
                }
                else
                {
                    bitmapImage = ImageConverter.ImageSourceForImageControl(_frame.ToBitmap());
                }
                _window.activyVideoPage.VideoImage.Source = bitmapImage;
                mediaSlider.Value = currentFrameNumber;
            }
            else
            {
                MessageBox.Show("Invalid frame number", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void GetProcessedVideo()
        {
            if (_window.activyVideoPage.IsProcessingVideoController) 
            {
                MessageBox.Show("Wait for the video processing to finish", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                IsProcessing = true;
                _window.activyVideoPage.IsProcessingVideoController = true;
            }
            Stopwatch stopwatch;
            try
            {
                if (ObjectsOnFrame == null)
                {
                    _window.activyVideoPage.StatusBox.Text = $"processing video {shortName}...";
                    stopwatch = Stopwatch.StartNew();
                    if (App.ContentFromConfig["ProcessInRealTime"] == "false")
                    {
                        ObjectsOnFrame = await MainWindow.apiClient.GetObjectsOnFrames(_videoCaptureForProcess, ConnectionWindow.ConnectionUri);
                    }
                    else
                    {
                        await MainWindow.apiClient.GetProcessedInRealTimeVideo(_videoCaptureForProcess, this, ConnectionWindow.ConnectionUri);
                    }
                    stopwatch.Stop();
                    Logger.LogByTemplate(LogEventLevel.Information, note: $"Video processed, frames count - {_countFrames}, frames processed - {ObjectsOnFrame.Count}, times - {stopwatch.ElapsedMilliseconds/1000}");
                    IsProcessed = true;
                    if (App.ContentFromConfig["AutoSave"] == "true")
                    {
                        SaveAllVideos(stopwatch);
                    }
                    else
                    {
                        MessageBox.Show("AutoSave if off,\n" +
                            $"Processed {ObjectsOnFrame.Count} frames,\n" +
                            $"time - {stopwatch.ElapsedMilliseconds/1000} s");
                    }
                }
                else
                {
                    if (ObjectsOnFrame.Count != _countFrames)
                    {
                        IsProcessed = false;
                        ObjectsOnFrame = null;
                        GetProcessedVideo();
                    }
                    else
                    {
                        IsProcessing = false;
                        _window.activyVideoPage.IsProcessingVideoController = false;
                        Logger.LogByTemplate(LogEventLevel.Warning, note: "An attempt to re-process the video.");
                        MessageBox.Show($"Video processed. frames - {ObjectsOnFrame.Count}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(LogEventLevel.Error, ex, note: "Video processing error.");
                _window.activyVideoPage.StatusBox.Text = "ERROR";
                IsProcessing = false;
                _window.activyVideoPage.IsProcessingVideoController = false;
            }
        }

        public void SaveVideo(string path, int startFrame, int endFrame)
        {
            if (startFrame < 0) startFrame = 0;
            if (endFrame > _videoCapture.FrameCount-1) endFrame = _videoCapture.FrameCount-1;

            try
            {
                _videoWriter = new(path, FourCC.FromString("FMP4"), _videoCapture.Fps, new OpenCvSharp.Size(_videoCapture.FrameWidth, _videoCapture.FrameHeight));
                Mat matFrame = new();
                for (int i = startFrame; i < endFrame; i++)
                {
                    _videoCaptureForProcess.Set(VideoCaptureProperties.PosFrames, i);
                    _videoCaptureForProcess.Read(matFrame);
                    if (i < ObjectsOnFrame?.Count)
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
                _window.activyVideoPage.StatusBox.Text = "ERROR";
            }
        }

        public void SaveAllVideos(Stopwatch timer)
        {
            _window.activyVideoPage.StatusBox.Text = "saving video segments";
            if (!IsProcessed) return;
            SelectFragmentsForSave();
            for (int i = 0; i < valuesForSave.Count; i++)
            {
                string path = FileHandler.SaveVideoFile(shortName, i, valuesForSave[i][0]);
                int keyframe = (valuesForSave[i][1] + valuesForSave[i][2])/2;
                int lenght = int.Parse(App.ContentFromConfig["ClipLength"]);
                SaveVideo(path, keyframe-lenght*_fps/2, keyframe+lenght*_fps/2);
            }
            _window.activyVideoPage.StatusBox.Text = "DONE";
            MessageBox.Show($"DONE, processed {ObjectsOnFrame.Count} frames,\n" +
                $"times - {timer.ElapsedMilliseconds / 1000} seconds,\n" +
                $"saved videos - {valuesForSave.Count}");
            IsProcessing = false;
            _window.activyVideoPage.IsProcessingVideoController = false;
        }
        private void SelectFragmentsForSave()
        {
            try
            {
                int[] bannedID = [];
                for (int i = 0; i < ObjectsOnFrame.Count; i++)
                {
                    int zebraCount = 0;
                    foreach (ObjectOnPhoto obj in ObjectsOnFrame[i])
                    {
                        if (obj.Class_name == "Zebra crossing")
                        {
                            zebraCount++;
                            if (zebraCount > 1)
                            {
                                bannedID.Append(obj.Class_id);
                                continue;
                            }
                            if (!bannedID.Contains(obj.Class_id))
                            {
                                if (valuesForSave.Count > 0)
                                {
                                    if (valuesForSave[^1][0] != obj.Class_id)
                                    {
                                        valuesForSave.Add([obj.Class_id, i, i]);
                                    }
                                    else
                                    {
                                        valuesForSave[^1][2] = i;
                                    }
                                }
                                else
                                {
                                    valuesForSave.Add([obj.Class_id, i, i]);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(LogEventLevel.Error, ex, "Error calculating fragments for save");
            }
        }

        public void Dispose()
        {
            if(IsProcessing) _window.activyVideoPage.IsProcessingVideoController = false;
        }

        #endregion
    }
}
