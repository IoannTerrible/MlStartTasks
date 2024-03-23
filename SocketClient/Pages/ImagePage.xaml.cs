using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SocketClient
{
    /// <summary>
    /// Логика взаимодействия для ImagePage.xaml
    /// </summary>
    public partial class ImagePage : Page
    {
        private MainWindow _window;
        public ImagePage(MainWindow window)
        {
            _window = window;
            InitializeComponent();
        }

        private void SendAndReciveButton_Click(object sender, RoutedEventArgs e)
        {
            ListBoxForResponce.Items.Clear();
            MainWindow.SendImage();
        }
        private async void HealthCheckButton_Click(object sender, RoutedEventArgs e)
        {
            await MainWindow.PerfomHealthChekAsync();
        }
    }
}
