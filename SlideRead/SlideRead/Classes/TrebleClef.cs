using System.Collections.Generic;

namespace SlideRead.Classes
{
    public class TrebleClef : IClef
    {
        public string MidNote { get; } = "B4"; //Note of middle line of this clef
        public List<string> MaxRange { get; } = new List<string>() { "F3", "F6" }; //For picking note
        public List<string> MinRangeSharp { get; } = new List<string>() { "A4", "G5" }; //For key signatures
        public List<string> MinRangeFlat { get; } = new List<string>() { "F4", "E5" }; //For key signatures
    }
}
