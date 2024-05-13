using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Net.Http;
using Newtonsoft.Json;
using Serilog;
using System.Windows.Threading;
using System.Windows.Media;

namespace Client
{
    public class HealthChecker : ApiClient
    {
        private readonly TextBox statusTextBox;
        private readonly MainWindow mainWindow;
        private bool isHealthy;

        public HealthChecker(MainWindow mainWindow, TextBox statusTextBox) : base(mainWindow)
        {
            this.mainWindow = mainWindow;
            this.statusTextBox = statusTextBox;
        }

        public async Task StartHealthCheckLoop(string apiUrl, int intervalInMillyseconds)
        {
            while (true)
            {
                await Task.Delay(intervalInMillyseconds); // Delay before the next health check
                isHealthy = await CheckHealthAsync(apiUrl);

                mainWindow.Dispatcher.Invoke(() =>
                {
                    statusTextBox.Text = isHealthy ? "Server is healthy" : "Server is not healthy";
                    statusTextBox.Foreground = isHealthy ? Brushes.Green : Brushes.Red;
                    mainWindow.isServerAlive = isHealthy;
                });
            }
        }
    }
}
