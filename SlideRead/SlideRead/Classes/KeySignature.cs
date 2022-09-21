using System.Collections.Generic;

namespace SlideRead.Classes
{
    public class KeySignature
    {
        public List<string> OrderOfFlat { get; } = new List<string>() { "Bb", "Eb", "Ab", "Db", "Gb", "Cb", "Fb" };
        public List<string> OrderOfSharp { get; } = new List<string>() { "F#", "C#", "G#", "D#", "A#", "E#", "B#" };
        public List<string> CMajorOctave { get; } = new List<string>() { "C", "D", "E", "F", "G", "A", "B" };
        public int OctaveNum { get; } = 7;
        public List<string> ConvertSharpFlat(List<string> GivenScale)
        {
            List<string> ls = new List<string>(GivenScale);
            foreach (string s in GivenScale)
            {
                int index = ls.IndexOf(s);
                if (s.Contains("b"))
                {
                    ls[index] = s.Replace("b", "#");
                    int newNote = CMajorOctave.IndexOf(s[0].ToString()) - 1;
                    if (newNote > -1)
                    {
                        ls[index] = ls[index].Replace(s[0], char.Parse(CMajorOctave[newNote]));
                    }
                    else
                    {
                        newNote += 7;
                        ls[index] = ls[index].Replace(s[0], char.Parse(CMajorOctave[newNote]));
                    }
                }
                else if (s.Contains("#"))
                {
                    ls[index] = s.Replace("#", "b");
                    int newNote = CMajorOctave.IndexOf(s[0].ToString()) + 1;
                    if (newNote < 7)
                    {
                        ls[index] = ls[index].Replace(s[0], char.Parse(CMajorOctave[newNote]));
                    }
                    else
                    {
                        newNote -= 7;
                        ls[index] = ls[index].Replace(s[0], char.Parse(CMajorOctave[newNote]));
                    }
                }
            }
            //string a = string.Join(", ", GivenScale);
            //string b = string.Join(", ", ls);
            //Console.WriteLine($"Old Scale: {a}\nNew Scale: {b}");
            return ls;
        }
    }
}
