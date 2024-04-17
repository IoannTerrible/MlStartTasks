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

namespace Client
{
    public class HealthChecker : ApiClient
    {
        private readonly TextBox statusTextBox;
        private readonly MainWindow mainWindow;

        public HealthChecker(MainWindow mainWindow, TextBox statusTextBox) : base(mainWindow)
        {
            this.mainWindow = mainWindow;
            this.statusTextBox = statusTextBox;
        }

        public async Task StartHealthCheckLoop(string apiUrl, int intervalInSeconds)
        {
            while (true)
            {
                await Task.Delay(intervalInSeconds * 1000); // Delay before the next health check
                bool isHealthy = await CheckHealthAsync(apiUrl);

                mainWindow.Dispatcher.Invoke(() =>
                {
                    statusTextBox.Text = isHealthy ? "Server is healthy" : "Server is not healthy";
                    mainWindow.isServerAlive = isHealthy;
                });
            }
        }
    }
}
