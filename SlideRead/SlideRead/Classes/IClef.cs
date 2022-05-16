using System;
using System.Collections.Generic;
using System.Text;

namespace SlideRead.Classes
{
    public interface IClef
    {
        string MidNote { get; }
        List<string> MaxRange { get; }
        List<string> MinRange { get; }
    }
}
