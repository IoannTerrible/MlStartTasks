using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для SaveWindow.xaml
    /// </summary>
    public partial class SaveWindow : Window
    {

        private static VideoController _videoController;
        private static MainWindow _window;
        private double _start;
        private double _end;
        private Ellipse _startPointEllipse;
        private Ellipse _endPointEllipse;
        private Line _lineBetweenPoints;
        public SaveWindow(MainWindow window, VideoController vCont)
        {
            InitializeComponent();
            _window = window;
            _videoController = vCont;

            VideoImage.Source = _window.activyVideoPage.VideoImage.Source;
            BindSlider();
        }
        private void MediaPlayButton_Click(object sender, RoutedEventArgs e) => _videoController?.Play();
        private void MediaPauseButton_Click(object sender, RoutedEventArgs e) => _videoController?.Pause();
        private void MediaStopButton_Click(object sender, RoutedEventArgs e) => _videoController?.Stop();
        private void RewindButton_Click(object sender, RoutedEventArgs e) => _videoController?.Rewind();
        private void NextButton_Click(object sender, RoutedEventArgs e) => _videoController?.NextFrame();
        private void ShowInfoButton_Click(object sender, RoutedEventArgs e) => _videoController?.ShowInfo();
        private void BindSlider()
        {
            MediaSliderForSave.Minimum = _videoController.mediaSlider.Minimum;
            MediaSliderForSave.Maximum = _videoController.mediaSlider.Maximum;
            MediaSliderForSave.Value = _videoController.mediaSlider.Value;
            MediaSliderForSave.ValueChanged += MediaSlider_ValueChanged;

            _videoController.mediaSlider.ValueChanged += (s, e) =>
            {
                MediaSliderForSave.Value = _videoController.mediaSlider.Value;
            };
        }
        private void MediaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _videoController.mediaSlider.Value = MediaSliderForSave.Value;
        }
        private void SetPointButton_Click(object sender, RoutedEventArgs e)
        {
            bool isStartPoint = sender == SetStartButton;

            // Проверка на нулевой диапазон
            double range = MediaSliderForSave.Maximum - MediaSliderForSave.Minimum;
            if (range == 0)
            {
                MessageBox.Show("Invalid slider range");
                return;
            }

            double thumbPosition = (MediaSliderForSave.Value - MediaSliderForSave.Minimum) / range * MediaSliderForSave.ActualWidth;
            GeneralTransform transform = MediaSliderForSave.TransformToAncestor(CanvasForPoints);
            Point sliderPosition = transform.Transform(new Point(0, 0));
            Point thumbCanvasPosition = new Point(sliderPosition.X + thumbPosition, sliderPosition.Y + (MediaSliderForSave.ActualHeight / 2));

            if (isStartPoint)
            {
                _start = MediaSliderForSave.Value;

                if (_startPointEllipse == null)
                {
                    _startPointEllipse = new Ellipse
                    {
                        Width = 4,
                        Height = 4,
                        Fill = Brushes.Red
                    };
                    CanvasForPoints.Children.Add(_startPointEllipse);
                }

                Canvas.SetLeft(_startPointEllipse, thumbCanvasPosition.X - _startPointEllipse.Width / 2.0);
                Canvas.SetTop(_startPointEllipse, thumbCanvasPosition.Y - _startPointEllipse.Height / 2.0);

                MessageBox.Show($"Start point set at {_start}");
            }
            else
            {
                _end = MediaSliderForSave.Value;

                if (_endPointEllipse == null)
                {
                    _endPointEllipse = new Ellipse
                    {
                        Width = 4,
                        Height = 4,
                        Fill = Brushes.Blue
                    };
                    CanvasForPoints.Children.Add(_endPointEllipse);
                }

                Canvas.SetLeft(_endPointEllipse, thumbCanvasPosition.X - _endPointEllipse.Width / 2);
                Canvas.SetTop(_endPointEllipse, thumbCanvasPosition.Y - _endPointEllipse.Height / 2);

                MessageBox.Show($"End point set at {_end}");
            }

            DrawLineBetweenPoints();
        }

        private void DrawLineBetweenPoints()
        {
            if (_startPointEllipse != null && _endPointEllipse != null)
            {
                Point startPoint = new Point(Canvas.GetLeft(_startPointEllipse) + _startPointEllipse.Width / 2,
                                             Canvas.GetTop(_startPointEllipse) + _startPointEllipse.Height / 2);
                Point endPoint = new Point(Canvas.GetLeft(_endPointEllipse) + _endPointEllipse.Width / 2,
                                           Canvas.GetTop(_endPointEllipse) + _endPointEllipse.Height / 2);

                if (_lineBetweenPoints == null)
                {
                    _lineBetweenPoints = new Line
                    {
                        Stroke = Brushes.Green,
                        StrokeThickness = 2
                    };
                    CanvasForPoints.Children.Add(_lineBetweenPoints);
                }

                _lineBetweenPoints.X1 = startPoint.X;
                _lineBetweenPoints.Y1 = startPoint.Y;
                _lineBetweenPoints.X2 = endPoint.X;
                _lineBetweenPoints.Y2 = endPoint.Y;
            }
        }

        private void SaveSegmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (_start < _end)
            {
                //SaveVideoSegment(_start, _end);
                MessageBox.Show($"{_start} {_end}");
            }
            else
            {
                MessageBox.Show("Invalid range. Start point must be less than end point.");
            }
        }

        public void VideoSource_Update()
        {
            VideoImage.Source = _window.activyVideoPage.VideoImage.Source;
        }

    }
}
