using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для StatisticsWindow.xaml
    /// </summary>
    public partial class StatisticsWindow : Window
    {
        public StatisticsWindow(ObservableCollection<LogEntry> logEntries)
        {
            InitializeComponent();

            var groupedEntries = logEntries
                .GroupBy(entry => entry.ObjectName)
                .Select(group => new
                {
                    ObjectName = group.Key,
                    Count = group.Count()
                })
                .OrderByDescending(entry => entry.Count);

            StackPanel stackPanel = new();

            foreach (var entry in groupedEntries)
            {
                TextBlock textBlock = new TextBlock
                {
                    Text = $"{entry.ObjectName}, Count: {entry.Count}",
                    Margin = new Thickness(10)
                };
                stackPanel.Children.Add(textBlock);
            }

            Content = stackPanel;
        }
    }
}

