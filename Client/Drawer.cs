using ClassLibrary;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Serilog.Events;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Client
{
    public class Drawer
    {
        private Canvas canvas;
        private readonly Image videoImage;
        private readonly Dictionary<string, Scalar> _classColors;
        private double scaleX;
        private double scaleY;
        MainWindow _window;
        private Dictionary<int, string> previousClassNames = new Dictionary<int, string>();

        public Drawer(Canvas canvas, Image videoImage, MainWindow window)
        {
            this._window = window;
            this.canvas = canvas;
            this.videoImage = videoImage;
            _classColors = new Dictionary<string, Scalar>
            {
            { "Additional signs", Scalar.Red },
            { "Car", Scalar.Green },
            { "Forbidding signs", Scalar.Blue },
            { "Information signs", Scalar.Yellow },
            { "Priority signs", Scalar.Cyan },
            { "Warning signs", Scalar.Magenta },
            { "Zebra crossing", Scalar.Orange }
            };
        }

        public Mat DrawBoundingBoxes(List<ObjectOnPhoto> aircraftObjects, Mat keyFrame)
        {
            Mat frame = keyFrame.Clone();

            foreach (var obj in aircraftObjects)
            {
                double xtl = obj.XTopLeftCorner;
                double ytl = obj.YTopLeftCorner;
                double xbr = obj.XBottonRigtCorner;
                double ybr = obj.YBottonRigtCorner;
                string name = obj.Class_name;
                int id = obj.Class_id;

                string metdate = $"{xtl},{ytl},{xbr},{ybr},{name},{id}";

                var videoController = _window.activyVideoPage.currentVideoController;
                var time = videoController.vtimer.FrameToTime(videoController.currentFrameNumber);
                var newEntry = new LogEntry(id.ToString(), name, time, "temp");

                if (!_window.activyVideoPage.LogEntries.Any(entry =>
                    entry.TrackId == newEntry.TrackId &&
                    entry.ObjectName == newEntry.ObjectName &&
                    entry.VideoTitle == newEntry.VideoTitle &&
                    entry.Timing == newEntry.Timing))
                {
                    _window.activyVideoPage.LogEntries.Add(newEntry);
                    _window.activyVideoPage.ListWithSqlResponce.ItemsSource = _window.activyVideoPage.LogEntries;
                }
                Logger.LogByTemplate(LogEventLevel.Debug, note: $"DrawObject with {xtl},{ytl}, {xbr}, {ybr}, {name}, {id}");
                DrawBoundingBox(frame, xtl, ytl, xbr, ybr, name, id);
            }
            return frame;
        }

        public void DrawBoundingBox(Mat frame, double xTopLeft, double yTopLeft, double xBottomRight, double yBottomRight, string name, int id)
        {
            Point topLeft = new Point((int)xTopLeft, (int)yTopLeft);
            Point bottomRight = new Point((int)xBottomRight, (int)yBottomRight);
            if (!_classColors.TryGetValue(name, out var color))
            {
                color = Scalar.White;
            }
            int thickness = 2;

                frame.Rectangle(topLeft, bottomRight, color, thickness);
            Cv2.PutText(frame, $"{name} {id}", topLeft, HersheyFonts.HersheySimplex, 1.0, color, thickness);
        }
        public void ClearRectangles()
        {
            canvas.Children.Clear();
        }

        public void CalculateScale()
        {
            if (videoImage.Source is BitmapSource)
            {
                BitmapSource bitmapSource = (BitmapSource)videoImage.Source;
                double currentWidth = videoImage.ActualWidth;
                double currentHeight = videoImage.ActualHeight;

                double originalWidth = bitmapSource.Width;
                double originalHeight = bitmapSource.Height;

                scaleX = currentWidth / originalWidth;
                scaleY = currentHeight / originalHeight;
            }
            Logger.LogByTemplate(LogEventLevel.Debug, note: "The image scale is calculated");
        }
    }
}
