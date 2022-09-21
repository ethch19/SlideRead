using System.Collections.Generic;

namespace SlideRead.Classes
{
    public class BassClef : IClef
    {
        public string MidNote { get; } = "D3"; //Note of middle line of this clef
        public List<string> MaxRange { get; } = new List<string>() { "E2", "A4" }; //For picking note
        public List<string> MinRangeSharp { get; } = new List<string>() { "A2", "G3" }; //For key signatures
        public List<string> MinRangeFlat { get; } = new List<string>() { "F2", "E3" }; //For key signatures
    }
}
