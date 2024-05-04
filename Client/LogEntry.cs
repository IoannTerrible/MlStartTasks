using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class LogEntry
    {
        public string UserName { get; set; }
        public string FileName { get; set; }
        public string FramePath { get; set; }
        public string MetaData { get; set; }
    }
}
