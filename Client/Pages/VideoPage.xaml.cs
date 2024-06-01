using ClassLibrary;
using Serilog.Events;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
namespace Client
{
    /// <summary>
    /// Логика взаимодействия для VideoPage.xaml
    /// </summary>
    public partial class VideoPage : Page
    {
        public VideoPage(MainWindow window)
        {
            InitializeComponent();
            _window = window;
            Canvas canvas = FindName("canvas2") as Canvas;
            canvas.Children.Add(rectangleContainer);

            ListWithSqlResponce.MouseDoubleClick += ListWithSqlResponce_MouseDoubleClick;

            ComboBoxForResponse.ItemsSource = OpenVideos;
            ClassFilterComboBox.ItemsSource = new string[] { "None", "Additional signs", "Car", "Forbidding signs", "Information signs", "Priority signs", "Warning signs", "Zebra crossing" };
            localDrawer = new Drawer(rectangleContainer, VideoImage, _window);
        }

        MainWindow _window;

        public readonly Drawer localDrawer;

        public ObservableCollection<string> OpenVideos { get; } = new ObservableCollection<string>();
        public VideoController currentVideoController;
        public SaveWindow saveWindow;
        public bool IsProcessingVideoController = false;

        private List<VideoController> _videoControllers = [];
        private readonly Canvas rectangleContainer = new();
        private string[] files;

        private void MediaPlayButton_Click(object sender, RoutedEventArgs e) => currentVideoController?.Play();
        private void MediaPauseButton_Click(object sender, RoutedEventArgs e) => currentVideoController?.Pause();
        private void MediaStopButton_Click(object sender, RoutedEventArgs e) => currentVideoController?.Stop();
        private void RewindButton_Click(object sender, RoutedEventArgs e) => currentVideoController?.Rewind();
        private void NextButton_Click(object sender, RoutedEventArgs e) => currentVideoController?.NextFrame();
        private void ShowInfoButton_Click(object sender, RoutedEventArgs e) => currentVideoController?.ShowInfo();

        private void MediaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider)sender).SelectionEnd = e.NewValue;
            currentVideoController?.GetSliderValue(e.NewValue);
        }
        private void UploadMediaButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                files = FileHandler.OpenFile(FileTypes.Media);
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        currentVideoController = new(file, VideoImage, MediaSlider, _window);
                        _videoControllers.Add(currentVideoController);
                        OpenVideos.Add($"{_videoControllers.Count}. {currentVideoController.shortName}");
                    }
                    ComboBoxForResponse.SelectedIndex = 0;
                    ComboBoxForResponse.UpdateLayout();


                    MediaSlider.Value = 0;
                }
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(LogEventLevel.Error, ex, note: "Media file opening error.");
                MessageBox.Show($"Media file opening error: {ex.Message}");
            }
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                currentVideoController?.GetProcessedVideo();
            }
            catch { }

        }
        private async void HealthCheckButton_Click(object sender, RoutedEventArgs e)
        {
            var healthStatus = await MainWindow.apiClient.CheckHealthAsync(ConnectionWindow.ConnectionUri);
            MessageBox.Show(healthStatus ? "Yes" : "No");
        }
        private void VideoBox_SourceUpdated(object sender, RoutedEventArgs e)
        {
            localDrawer.CalculateScale();
            localDrawer.ClearRectangles();
            Logger.LogByTemplate(LogEventLevel.Information, note: $"Image uploaded.");

        }
        private void ComboBoxForResponse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox.SelectedItem != null)
            {
                currentVideoController.IsPaused = true;
                currentVideoController = _videoControllers[comboBox.SelectedIndex];
                currentVideoController.SetFirstFrame();
                ListWithSqlResponce.ItemsSource = currentVideoController.logEntries;
            }
            else
            {
                currentVideoController = null;
                VideoImage.Source = null;
                _window.activyVideoPage.localDrawer.ClearRectangles();
            }
        }
        private async void ListWithSqlResponce_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListWithSqlResponce.SelectedItem is LogEntry selectedLogEntry)
            {
                try
                {
                    currentVideoController.SetFrame(int.Parse(selectedLogEntry.VideoTitle));
                }
                catch (Exception ex)
                {
                    Logger.LogByTemplate(LogEventLevel.Error, ex);
                }
            }
        }
        private void ClassFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedClassItem = ClassFilterComboBox.SelectedItem.ToString();
            if (selectedClassItem != null)
            {
                if (currentVideoController.filteredLogEntries != null) currentVideoController.filteredLogEntries.Clear();
                foreach (var entry in currentVideoController.logEntries.Where(le => le.ObjectName == selectedClassItem))
                {
                    currentVideoController.filteredLogEntries.Add(entry);
                }
                ListWithSqlResponce.ItemsSource = currentVideoController.filteredLogEntries;
                if (selectedClassItem == "None") ListWithSqlResponce.ItemsSource = currentVideoController.logEntries;
            }
            else
            {
                if (currentVideoController.filteredLogEntries != null) currentVideoController.filteredLogEntries.Clear();
                foreach (var entry in currentVideoController.logEntries)
                {
                    currentVideoController.filteredLogEntries.Add(entry);
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxForResponse.Items.Count == 0) return;
            if (ComboBoxForResponse.Items.Count == 1)
            {
                currentVideoController.Dispose();
                ComboBoxForResponse.SelectedIndex = -1;
                _videoControllers.Clear();
                OpenVideos.Clear();
                return;
            }
            currentVideoController.Dispose();
            int selectedIndex = ComboBoxForResponse.SelectedIndex;
            ComboBoxForResponse.SelectedIndex = selectedIndex > 0 ? selectedIndex - 1 : 0;
            _videoControllers.RemoveAt(selectedIndex);
            OpenVideos.RemoveAt(selectedIndex);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentVideoController == null) return;
            if (currentVideoController.IsProcessed)
            {
                saveWindow = new(_window, currentVideoController);
                if (saveWindow.ShowDialog() == true)
                {
                    MessageBox.Show("success save", "complete", MessageBoxButton.OK);
                }
            }
            else
            {
                MessageBox.Show("you need process video before save", "save error", MessageBoxButton.OK);
            }
        }

        private void StatsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (currentVideoController != null && currentVideoController.logEntries != null && currentVideoController.IsProcessed)
                {
                    StatisticsWindow statisticsWindow = new StatisticsWindow(currentVideoController.logEntries);
                    statisticsWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("You need to process video");
                }
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(LogEventLevel.Error, ex, "Error with Stats");
            }
        }
    }
}
