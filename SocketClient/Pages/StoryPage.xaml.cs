using Accessibility;
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
using System.Windows.Threading;

namespace SocketClient
{
    /// <summary>
    /// Логика взаимодействия для StoryPage.xaml
    /// </summary>
    public partial class StoryPage : Page
    {
        internal List<string> lines;
        internal float delay;
        private App _app;
        private MainWindow _mainWindow;
        private bool receivingLines = false;
        public StoryPage(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            InitializeComponent();
            _app = (App)Application.Current;
            delay = float.Parse(App.ContentFromConfig[2]);

        }
        public async Task AddLineToListBoxWithDelay(string line)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                this.StoryListBox.Items.Add(line);
            }, DispatcherPriority.Background);
            await Task.Delay(TimeSpan.FromSeconds(delay));
        }

        private async void Go_Click(object sender, RoutedEventArgs e)
        {
            if (!receivingLines) // Если прием строк с сервера не идет в данный момент
            {
                receivingLines = true; // Устанавливаем флаг, что начат прием строк с сервера
                await _mainWindow.SendMessageAndReceive("LOR");
                receivingLines = false; // Сбрасываем флаг приема строк с сервера
            }
            else
            {
                // Если прием строк с сервера уже идет, показываем сообщение об этом
                MessageBox.Show("Получение строк с сервера уже запущено.");
            }
        }
    }
}
