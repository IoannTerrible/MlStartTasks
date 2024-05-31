using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;

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
        private void SetStartButton_Click(object sender, RoutedEventArgs e)
        {
            _start = MediaSliderForSave.Value;
            MessageBox.Show($"Start point set at {_start}");
        }

        private void SetEndButton_Click(object sender, RoutedEventArgs e)
        {
            _end = MediaSliderForSave.Value;
            MessageBox.Show($"End point set at {_end}");
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
