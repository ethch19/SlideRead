using System;
using System.Collections.Generic;
using System.Text;

namespace SlideRead.Classes
{
    public class TenorClef : IClef
    {
        public string MidNote { get; } = "A3"; //Note of middle line of this clef
        public List<string> MaxRange { get; } = new List<string>() {"G2", "B4"}; //For picking note
        public List<string> MinRange { get; } = new List<string>() { "F3", "E4"}; //For key signatures
    }
}
