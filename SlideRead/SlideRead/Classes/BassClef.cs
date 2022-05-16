using System;
using System.Collections.Generic;
using System.Text;

namespace SlideRead.Classes
{
    public class BassClef : IClef
    {
        public string MidNote { get; } = "D3"; //Note of middle line of this clef
        public List<string> MaxRange { get; } = new List<string>() {"E2", "A4"}; //For picking note
        public List<string> MinRange { get; } = new List<string>() { "F2", "G3"}; //For key signatures
    }
}
