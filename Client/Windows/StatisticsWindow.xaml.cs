using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

            StackPanel stackPanel = new StackPanel();

            foreach (var entry in groupedEntries)
            {
                TextBlock textBlock = new TextBlock
                {
                    Text = $"ObjectName: {entry.ObjectName}, Count: {entry.Count}",
                    Margin = new Thickness(10)
                };
                stackPanel.Children.Add(textBlock);
            }

            Content = stackPanel;
        }
    }
}

