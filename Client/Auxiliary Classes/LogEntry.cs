using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class LogEntry
    {
        public string TrackId { get; set; }
        public string ObjectName { get; set; } 
        public string Timing { get; set; } 
        public string VideoTitle { get; set; } 
        public LogEntry(string fileName, string objectName, string timing, string videoTitle)
        {
            TrackId = fileName;
            ObjectName = objectName;
            Timing = timing;
            VideoTitle = videoTitle;
        }
    }


}
