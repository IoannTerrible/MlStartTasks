using Serilog.Events;
using SocketClient;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ClassLibrary;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Client
{
    public class Drawer
    {
        private Canvas canvas;
        private readonly Image videoImage;
        private double scaleX;
        private double scaleY;
        public Drawer(Canvas canvas, Image videoImage)
        {
            this.canvas = canvas;
            this.videoImage = videoImage;
        }

        public void DrawBoundingBoxes(List<ObjectOnPhoto> aircraftObjects)
        {
            foreach (var obj in aircraftObjects)
            {
                double xtl = obj.XTopLeftCorner;
                double ytl = obj.YTopLeftCorner;
                double xbr = obj.XBottonRigtCorner;
                double ybr = obj.YBottonRigtCorner;
                string name = obj.Class_name;
                DrawBoundingBox(xtl, ytl, xbr, ybr, name);
            }
            Logger.LogByTemplate(LogEventLevel.Information, note: $"{aircraftObjects.Count} borders of objects have been drawn.");
        }

        private void DrawBoundingBox(double xTopLeft, double yTopLeft, double xBottomRight, double yBottomRight, string name)
        {
            Rectangle boundingBox = new Rectangle();

            // Calculate scaled coordinates here using the actual image size and calculated scale
            double scaledXTopLeft = xTopLeft * scaleX;
            double scaledYTopLeft = yTopLeft * scaleY;
            double scaledWidth = (xBottomRight - xTopLeft) * scaleX;
            double scaledHeight = (yBottomRight - yTopLeft) * scaleY;

            boundingBox.Width = scaledWidth;
            boundingBox.Height = scaledHeight;
            if (canvas != null)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = name;
                textBlock.FontSize = 16;
                textBlock.Foreground = Brushes.Red;
                Canvas.SetLeft(textBlock, scaledXTopLeft);
                Canvas.SetTop(textBlock, scaledYTopLeft);
                canvas.Children.Add(textBlock);
                Canvas.SetLeft(boundingBox, scaledXTopLeft);
                Canvas.SetTop(boundingBox, scaledYTopLeft);
                Canvas.SetZIndex(boundingBox, int.MaxValue);

                boundingBox.Stroke = Brushes.Red;
                boundingBox.StrokeThickness = 2;
                boundingBox.Fill = Brushes.Transparent;

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
