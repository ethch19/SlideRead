using System.Collections.Generic;

namespace SlideRead.Classes
{
    public class TromboneConfig
    {
        public int CenOct { get; set; }
        public IList<IList<int>> EnlargeRange { get; set; }
        public IList<string> FlatOctave { get; set; }
        public IList<string> SharpOctave { get; set; }
        public IList<int> MaxPos { get; set; }
    }
}
