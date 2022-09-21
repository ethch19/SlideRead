using System.Collections.Generic;

namespace SlideRead.Classes
{
    public interface IClef
    {
        string MidNote { get; }
        List<string> MaxRange { get; }
        List<string> MinRangeSharp { get; }
        List<string> MinRangeFlat { get; }
    }
}
