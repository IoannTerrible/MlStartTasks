using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Client.App app = new Client.App();
            app.InitializeComponent();
            app.Run();
        }
        //public async Task ProcessLinesInBackground(StoryLinePage storyPage)
        //{
        //    await Task.Run(() =>
        //    {
        //        Application.Current.Dispatcher.Invoke(() =>
        //        {
        //            if (storyPage != null)
        //            {
        //                storyPage.StartProcessingLines(MlStartTask2.Program.lines, Program.GetDelayInSeconds());
        //            }
        //        });
        //    });
        //}
    }
}
