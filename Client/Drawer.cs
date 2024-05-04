using Serilog.Events;

using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ClassLibrary;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Data.SqlClient;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace Client
{
    public class Drawer
    {
        private Canvas canvas;
        private readonly Image videoImage;
        private double scaleX;
        private double scaleY;
        MainWindow _window;
        private Dictionary<int, string> previousClassNames = new Dictionary<int, string>();

        public Drawer(Canvas canvas, Image videoImage, MainWindow window)
        {
            this._window = window;
            this.canvas = canvas;
            this.videoImage = videoImage;
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

                if (IsClassNameChanged(id, name))
                {
                    Logger.LogByTemplate(LogEventLevel.Information, note: $"Class_name for object with id {id} changed to {name}");
                    FileHandler.SaveBitmapImageToFile(frame.ToBitmap());
                    CreateEventLogEntry(LogInPage.login, _window.activyVideoPage.currentVideoController.shortName, FileHandler.LastKeyFrameName, metdate, MainWindow.connectionString);
                }

                DrawBoundingBox(frame, xtl, ytl, xbr, ybr, name, id);
                Logger.LogByTemplate(LogEventLevel.Debug, note: $"DrawObject with {xtl},{ytl}, {xbr}, {ybr}, {name}, {id}");
            }

            Logger.LogByTemplate(LogEventLevel.Information, note: $"{aircraftObjects.Count} borders of objects have been drawn.");

            return frame;
        }

        public void DrawBoundingBox(Mat frame, double xTopLeft, double yTopLeft, double xBottomRight, double yBottomRight, string name, int id)
        {
            Point topLeft = new Point((int)xTopLeft, (int)yTopLeft);
            Point bottomRight = new Point((int)xBottomRight, (int)yBottomRight);
            Scalar color = Scalar.Red;
            int thickness = 2;

            frame.Rectangle(topLeft, bottomRight, color, thickness);
            Cv2.PutText(frame, $"{name} {id}", topLeft, HersheyFonts.HersheySimplex, 1.0, color, thickness);
        }
        private void CreateEventLogEntry(string userName,
            string fileName, string framePath, string metaData, string sqlConnectionString)
        {
            string insertQuery = $"INSERT INTO EventLog (UserName, FileName, FramePath, MetaDate) " +
                                 $"VALUES ('{userName}', '{fileName}', '{framePath}', '{metaData}')";
            SqlCommand sqlCommand = new SqlCommand(insertQuery);

            SqlCore.ExecuteSQL(sqlCommand, sqlConnectionString);
        }
        private bool IsClassNameChanged(int id, string newClassName)
        {
            if (previousClassNames.ContainsKey(id))
            {
                string previousClassName = previousClassNames[id];
                if (previousClassName != newClassName)
                {
                    previousClassNames[id] = newClassName;
                    return true;
                }
            }
            else
            {
                previousClassNames[id] = newClassName;
            }

            return false;
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
