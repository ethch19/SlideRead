using System;
using System.Collections.Generic;
using System.Text;

namespace SlideRead.Classes
{
    public class TrebleClef : IClef
    {
        public string MidNote { get; } = "B4"; //Note of middle line of this clef
        public List<string> MaxRange { get; } = new List<string>() {"F3", "F6"}; //For picking note
        public List<string> MinRange { get; } = new List<string>() { "A4", "G5"}; //For key signatures
    }
}
