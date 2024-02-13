using System;
using System.Collections.Generic;

namespace ClassLibraryOne
{
    public class UiAndMainConnector
    {
        public event EventHandler<List<string>> LinesUpdated;
        public List<string> freshLines;
        public void GetLines(List<string> lines)
        {
            var linesList = new List<string>(lines);

            OnLinesUpdated(linesList);
        }
        public void AddFreshLines(List<string> lines) { freshLines = lines; }
        public List<string> GetFreshLines()
        {
            return freshLines;
        }

        protected virtual void OnLinesUpdated(List<string> lines)
        {
            LinesUpdated?.Invoke(this, lines);
        }
        
    }
}
