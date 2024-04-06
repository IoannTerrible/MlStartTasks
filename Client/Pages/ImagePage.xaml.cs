using ClassLibrary;
using Serilog.Events;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace SocketClient
{
    /// <summary>
    /// Логика взаимодействия для ImagePage.xaml
    /// </summary>
    public partial class ImagePage : Page
    {
        #region Constructor
        public ImagePage(MainWindow window)
        {
            _window = window;
            InitializeComponent();
            canvas.Children.Add(rectangleContainer);
        }
        #endregion
        #region Attributes
        private readonly MainWindow _window;
        private readonly Canvas rectangleContainer = new();

        private double originalWidth;
        private double originalHeight;
        private double scaleX;
        private double scaleY;
        #endregion
        #region Public Methods
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
        #endregion
        #region Private Methods
        private void SendAndReciveButton_Click(object sender, RoutedEventArgs e)
        {
            ListBoxForResponce.Items.Clear();
            MainWindow.SendImage();
            ClearRectangles();
        }
        private async void HealthCheckButton_Click(object sender, RoutedEventArgs e)
        {
            await MainWindow.PerfomHealthChekAsync();
        }
        private void ClearRectangles()
        {
            rectangleContainer.Children.Clear();
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
        private void ImageBoxThree_SourceUpdated(object sender, RoutedEventArgs e)
        {
            CalculateScale();
            ClearRectangles();
            Logger.LogByTemplate(LogEventLevel.Information, note: $"Image uploaded.");
        }
        private void CalculateScale()

        {
            if (ImageBoxThree.Source is BitmapSource)
            {
                BitmapSource bitmapSource = (BitmapSource)ImageBoxThree.Source;
                double currentWidth = ImageBoxThree.ActualWidth;
                double currentHeight = ImageBoxThree.ActualHeight;

                originalWidth = bitmapSource.Width;
                originalHeight = bitmapSource.Height;

                scaleX = currentWidth / originalWidth;
                scaleY = currentHeight / originalHeight;
            }
            Logger.LogByTemplate(LogEventLevel.Debug, note: "The image scale is calculated");
        }
        #endregion
    }
}