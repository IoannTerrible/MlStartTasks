using ClassLibraryOne;
using Microsoft.VisualBasic;
using MlStartTask2;
using Serilog;
using System.Data;
using Serilog.Events;
using System.Diagnostics;
using System.Windows;
using System.Xml;
using static Serilog.Events.LogEventLevel;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace WpfApp1
{
    /////// <summary>
    /////// Interaction logic for App.xaml
    /////// </summary>
    public partial class App : Application
    {
        public List<string> lines { get; set; }
        [STAThread]
        public static void Main(string[] args)
        {
            WpfApp1.App app = new WpfApp1.App();
            app.InitializeComponent();
            app.Run();
        }

        public async Task ProcessLinesInBackground(StoryLinePage storyPage)
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (storyPage != null)
                    {
                        storyPage.StartProcessingLines(MlStartTask2.Program.lines, Program.GetDelayInSeconds()); 
                    }
                });
            });
        }
    }
}


