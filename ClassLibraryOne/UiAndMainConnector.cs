using System;
using System.Collections.Generic;

namespace ClassLibraryOne
{
    public class UiAndMainConnector
    {
        public event EventHandler<List<string>> LinesUpdated;
        public void GetLines(List<string> lines)
        {
            var linesList = new List<string>(lines);

            OnLinesUpdated(linesList);
        }

        protected virtual void OnLinesUpdated(List<string> lines)
        {
            LinesUpdated?.Invoke(this, lines);
        }
    }
}
