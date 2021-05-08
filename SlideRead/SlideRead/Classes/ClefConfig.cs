using System;
using System.Collections.Generic;
using System.Text;

namespace SlideRead.Classes
{
    public class ClefConfig
    {
        public IList<string> Bass { get; set; }
        public IList<string> Treble { get; set; }
        public IList<string> Tenor { get; set; }
        public IList<string> OrderOfFlat { get; set; }
        public IList<string> OrderOfSharp { get; set; }
    }
}
