using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xamarin.Essentials;

namespace SlideRead.Classes
{
    public enum Clef
    {
        Treble,
        Bass,
        Tenor,
        Mixed
    }
    public enum Accidentals
    {
        Sharp,
        Flat,
        Mixed,
        None
    }
    public enum Key
    {
        Sharp,
        Flat,
        Neutral
    }
    public class Settings
    {
        public int questions { get; set; } = 15;
        public int timelimit { get; set; } = 10;
        public Clef clef { get; set; } = Clef.Bass;
        public string displaykey { get; set; } = "C Major or A Minor";
        public Key keyFlag { get; set; } = Key.Neutral;
        public int numOfKey { get; set; } = 0;
        public Accidentals accidentals { get; set; } = Accidentals.None;

        public Settings()
        {
/*            Preferences.Clear();*/
            foreach (PropertyInfo propertyInfo in this.GetType().GetProperties())
            {
                if (Preferences.ContainsKey(propertyInfo.Name))
                {
                    switch (propertyInfo.Name)
                    {
                        case "questions":
                            propertyInfo.SetValue(this, Int32.Parse(Preferences.Get(propertyInfo.Name, null)));
                            break;
                        case "timelimit":
                            propertyInfo.SetValue(this, Int32.Parse(Preferences.Get(propertyInfo.Name, null)));
                            break;
                        case "clef":
                            propertyInfo.SetValue(this, Enum.Parse(typeof(Clef), Preferences.Get(propertyInfo.Name, null)));
                            break;
                        case "displaykey":
                            propertyInfo.SetValue(this, Preferences.Get(propertyInfo.Name, null));
                            break;
                        case "keyFlag":
                            propertyInfo.SetValue(this, Enum.Parse(typeof(Key), Preferences.Get(propertyInfo.Name, null)));
                            break;
                        case "numOfKey":
                            propertyInfo.SetValue(this, Int32.Parse(Preferences.Get(propertyInfo.Name, null)));
                            break;
                        case "accidentals":
                            propertyInfo.SetValue(this, Enum.Parse(typeof(Accidentals), Preferences.Get(propertyInfo.Name, null)));
                            break;
                        default:
                            propertyInfo.SetValue(this, Preferences.Get(propertyInfo.Name, null));
                            break;
                    }
                }
            }
        }
        public void SaveSettings()
        {
            foreach (PropertyInfo propertyInfo in this.GetType().GetProperties())
            {
                if (Preferences.ContainsKey(propertyInfo.Name) == false || Preferences.Get(propertyInfo.Name, null).ToString() != propertyInfo.GetValue(this).ToString())
                {
                    Preferences.Set(propertyInfo.Name, propertyInfo.GetValue(this).ToString());
                }
            }
        }
    }
}
