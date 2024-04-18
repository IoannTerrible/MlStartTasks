using Serilog.Events;

using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ClassLibrary;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Data.SqlClient;
using OpenCvSharp;

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

        public void DrawBoundingBoxes(List<ObjectOnPhoto> aircraftObjects, System.Drawing.Bitmap keyFrame)
        {
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
                    FileHandler.SaveBitmapImageToFile(keyFrame);
                    CreateEventLogEntry(LogInPage.login, _window.activyVideoPage.currentVideoController.shortName, FileHandler.LastKeyFrameName, metdate, MainWindow.connectionString);
                }
                DrawBoundingBox(xtl, ytl, xbr, ybr, name, id);
                Logger.LogByTemplate(LogEventLevel.Debug, note: $"DrawObject with {xtl},{ytl}, {xbr}, {ybr}, {name}, {id}");
            }

            Logger.LogByTemplate(LogEventLevel.Information, note: $"{aircraftObjects.Count} borders of objects have been drawn.");
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
        public void DrawBoundingBox(double xTopLeft, double yTopLeft, double xBottomRight, double yBottomRight, string name, int id, bool itIsLog = false)
        {
            Rectangle boundingBox = new Rectangle();

            double scaledXTopLeft = xTopLeft * scaleX;
            double scaledYTopLeft = yTopLeft * scaleY;
            double scaledWidth = (xBottomRight - xTopLeft) * scaleX;
            double scaledHeight = (yBottomRight - yTopLeft) * scaleY;

            boundingBox.Width = scaledWidth;
            boundingBox.Height = scaledHeight;
            if (canvas != null)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = $"{name} {id}";
                textBlock.FontSize = 12;
                textBlock.Foreground = Brushes.Red;
                Canvas.SetLeft(textBlock, scaledXTopLeft);
                Canvas.SetTop(textBlock, scaledYTopLeft);

                Canvas.SetLeft(boundingBox, scaledXTopLeft);
                Canvas.SetTop(boundingBox, scaledYTopLeft);
                Canvas.SetZIndex(boundingBox, int.MaxValue);
                if(name == "fall")
                {
                    boundingBox.Stroke = Brushes.Red;
                    textBlock.Foreground = Brushes.Cyan;

                }
                else
                {
                    boundingBox.Stroke = Brushes.Cyan;
                    textBlock.Foreground = Brushes.Red;

                }
                boundingBox.StrokeThickness = 1;
                boundingBox.Fill = Brushes.Transparent;
                if(itIsLog == false)
                {
                    canvas.Children.Add(textBlock);
                }
                canvas.Children.Add(boundingBox);
            }
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
