using System;
using System.Collections.Generic;
using System.Text;

namespace SlideRead.Classes
{
    public class GeneralConfig
    {
        public IList<int> CenOct { get; set; }
        public IList<IList<int>> EnlargeRange { get; set; }
        public IList<string> FlatOctave { get; set; }
        public IList<string> SharpOctave { get; set; }
    }
}
