using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SlideRead.Pages
{
    public partial class SettingsPage : ContentPage
    {
        Dictionary<String, Guid> ControlsIDDictionary = new Dictionary<string, Guid>();
        Controls.ScrollList currentScrollList;
        Dictionary<String, Dictionary<Controls.DropdownMenu, Controls.DropdownButton>> DropdownControlsDictionary = new Dictionary<string, Dictionary<Controls.DropdownMenu, Controls.DropdownButton>>();
        Classes.Settings settings = new Classes.Settings();
        Dictionary<String, List<String>> SelectionPossibilities = new Dictionary<string, List<string>>()
        {
            {"questions", new List<string>(){ "10", "15", "20"} },
            {"timelimit", new List<string>(){ "5", "10", "15"} },
            {"clef", new List<string>(){ "Treble", "Bass", "Tenor", "Mixed"} },
            {"accidentals", new List<string>(){ "Sharp", "Flat", "Mixed", "None"} },
        };
        Dictionary<String, List<String>> keyDictionary = new Dictionary<string, List<String>>()
        {
            { "C", new List<string>(){ "Neutral", "0"} },
            { "G", new List<string>(){ "Sharp", "1"} },
            { "D", new List<string>(){ "Sharp", "2"} },
            { "A", new List<string>(){ "Sharp", "3"} },
            { "E", new List<string>(){ "Sharp", "4"} },
            { "B", new List<string>(){ "Sharp", "5"} },
            { "Cb", new List<string>(){ "Flat", "7"} },
            { "F", new List<string>(){ "Flat", "1"} },
            { "Bb", new List<string>(){ "Flat", "2"} },
            { "Eb", new List<string>(){ "Flat", "3"} },
            { "Ab", new List<string>(){ "Flat", "4"} },
            { "Db", new List<string>(){ "Flat", "5"} },
            { "C#", new List<string>(){ "Sharp", "7"} },
            { "Gb", new List<string>(){ "Flat", "6"} },
            { "F#", new List<string>(){ "Sharp", "6"} }
        };
        public SettingsPage()
        {
            InitializeComponent();
            FakeButton1.Text = settings.questions.ToString();
            FakeButton2.Text = settings.timelimit.ToString();
            FakeButton3.Text = settings.clef.ToString();
            FakeButton5.Text = settings.accidentals.ToString();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            topGrid.Opacity = 0;
            topGrid.FadeTo(1, 350);
        }
        private async void BackBtnClicked(object sender, EventArgs args)
        {
            await Application.Current.MainPage.Navigation.PopAsync(false);
        }
        private void ScrollListClicked(object sender, EventArgs args)
        {
            Button btn = (Button)sender;
            if (btn.IsEnabled == false) return;
            btn.IsEnabled = false;
            DarkenedLayer.IsVisible = true;
            scrollListExitBtn.IsVisible = true;
            Controls.ScrollList scrollList = new Controls.ScrollList()
            {
                StyleId = "scrollList"
            };
            currentScrollList = scrollList;
            currentScrollList.Content.FindByName<Label>("CurrentItemLabel").Text = settings.GetType().GetProperty("displaykey").GetValue(settings).ToString();
            topGrid.Children.Add(scrollList, 0, 0);
            topGrid.RaiseChild(scrollListExitBtn);
            btn.IsEnabled = true;
        }
        private void CloseScrollList(object sender, EventArgs args)
        {
            ImageButton btn = (ImageButton)sender;
            if (btn.IsEnabled == false) return;
            btn.IsEnabled = false;
            string keySelection = currentScrollList.Content.FindByName<Label>("CurrentItemLabel").Text;
            settings.GetType().GetProperty("displaykey").SetValue(settings, keySelection);
            string[] splitKeySelection = keySelection.Split();
            Console.WriteLine(splitKeySelection[0]);
            List<String> tempInfo = keyDictionary.FirstOrDefault(x => x.Key == splitKeySelection[0]).Value;
            settings.GetType().GetProperty("keyFlag").SetValue(settings, Enum.Parse(typeof(Classes.Key), tempInfo[0]));
            settings.GetType().GetProperty("numOfKey").SetValue(settings, Int32.Parse(tempInfo[1]));
            Console.WriteLine(tempInfo[0]);
            Console.WriteLine(tempInfo[1]);
            settings.SaveSettings();
            topGrid.Children.Remove(currentScrollList);
            DarkenedLayer.IsVisible = false;
            btn.IsVisible = false;
            btn.IsEnabled = true;
        }
        private void DropDownClicked(object sender, EventArgs args)
        {
            Button btn = (Button)sender;
            if (btn.IsEnabled == false)
            {
                return;
            }
            btn.IsEnabled = false;
            string dynamicSetting = null;
            int interval = 0;
            bool dropped = false;
            Console.WriteLine(btn.Id);
            {
                foreach (Guid item in ControlsIDDictionary.Values)
                {
                    if (item == btn.Id)
                    {
                        Console.WriteLine("found");
                    }
                }
            }
            // Check if there is a dropdown menu already or not
            if (btn.Id == FakeButton1.Id)
            {
                dynamicSetting = "questions";
                interval = 1;
            }
            else if (btn.Id == FakeButton2.Id)
            {
                dynamicSetting = "timelimit";
                interval = 2;
            }
            else if (btn.Id == FakeButton3.Id)
            {
                dynamicSetting = "clef";
                interval = 3;
            }
            else if (btn.Id == FakeButton5.Id)
            {
                dynamicSetting = "accidentals";
                interval = 5;
            }
            else
            {
                // Get the interval and dynamic setting for a DROPPED down menu
                foreach (KeyValuePair<String, Guid> current in ControlsIDDictionary)
                {
                    if (current.Value != btn.Id)
                    {
                        continue;
                    }
                    string c = current.Key.Replace("dropdownButton", "");
                    int intervalString = Int32.Parse(c);
                    interval = intervalString;
                }
                switch (interval)
                {
                    case 1:
                        dynamicSetting = "questions";
                        break;
                    case 2:
                        dynamicSetting = "timelimit";
                        break;
                    case 3:
                        dynamicSetting = "clef";
                        break;
                    case 5:
                        dynamicSetting = "accidentals";
                        break;
                }
                dropped = true;
            }
            // If there is already a dropdown menu, deletes it
            if (dropped == true)
            {
                Content.FindByName<Button>("FakeButton" + interval.ToString()).Text = settings.GetType().GetProperty(dynamicSetting).GetValue(settings, null).ToString();
                Content.FindByName<Button>("FakeButton" + interval.ToString()).IsEnabled = true;
                Content.FindByName<Image>("Fake" + interval.ToString()).IsVisible = true;
                foreach (View item in topGrid.Children)
                {
                    Console.WriteLine(item.StyleId);
                }
                topGrid.Children.Remove(DropdownControlsDictionary.FirstOrDefault(x => x.Key == ("ExpandedBase" + interval.ToString())).Value.FirstOrDefault(y => y.Key.StyleId == ("base" + interval.ToString())).Key);
                topGrid.Children.Remove(DropdownControlsDictionary.FirstOrDefault(x => x.Key == ("ExpandedBase" + interval.ToString())).Value.FirstOrDefault(y => y.Value.StyleId == ("button" + interval.ToString())).Value);
                DropdownControlsDictionary.Remove("ExpandedBase" + interval.ToString());
                ControlsIDDictionary.Remove("base" + interval.ToString());
                ControlsIDDictionary.Remove("dropdownButton" + interval.ToString());
                ControlsIDDictionary.Remove("selection1button" + interval.ToString());
                ControlsIDDictionary.Remove("selection2button" + interval.ToString());
                if (interval == 3 || interval == 5)
                {
                    ControlsIDDictionary.Remove("selection3button" + interval.ToString());
                }
                return;
            }
            // Create a dropdown menu and button IF there isn't one
            double parentX = (mainGrid.X + stackGrid.X + Content.FindByName<StackLayout>("stackRow" + interval.ToString()).X + Content.FindByName<Grid>("button" + interval.ToString() + "Grid").X + Content.FindByName<Image>("Fake" + interval.ToString()).X);
            double parentY = (mainGrid.Y + stackGrid.Y + Content.FindByName<StackLayout>("stackRow" + interval.ToString()).Y + Content.FindByName<Grid>("button" + interval.ToString() + "Grid").Y + Content.FindByName<Image>("Fake" + interval.ToString()).Y);
            Controls.DropdownMenu tempDropdownMenu;
            Controls.DropdownButton tempDropdownButton;
            //Need to add an extra selection to the dropdown menu
            if (interval == 3 || interval == 5)
            {
                tempDropdownMenu = new Controls.DropdownMenu(3)
                {
                    AnchorX = 0,
                    AnchorY = 0,
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.Start,
                    Margin = new Thickness(parentX, (parentY - 3), 0, 0),
                    StyleId = ("base" + interval.ToString())
                };
                tempDropdownButton = new Controls.DropdownButton()
                {
                    AnchorX = 0,
                    AnchorY = 0,
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.Start,
                    Margin = new Thickness(parentX, (parentY - 3), 0, 0),
                    StyleId = ("button" + interval.ToString())
                };
            }
            else
            {
                tempDropdownMenu = new Controls.DropdownMenu()
                {
                    AnchorX = 0,
                    AnchorY = 0,
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.Start,
                    Margin = new Thickness(parentX, (parentY - 3), 0, 0),
                    StyleId = ("base" + interval.ToString())
                };
                tempDropdownButton = new Controls.DropdownButton()
                {
                    AnchorX = 0,
                    AnchorY = 0,
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.Start,
                    Margin = new Thickness(parentX, (parentY - 3), 0, 0),
                    StyleId = ("button" + interval.ToString())
                };
            }
            Console.WriteLine("ExpandedBase" + interval.ToString());
            DropdownControlsDictionary.Add(("ExpandedBase" + interval.ToString()), new Dictionary<Controls.DropdownMenu, Controls.DropdownButton>(){{ tempDropdownMenu, tempDropdownButton}});
            Console.WriteLine(DropdownControlsDictionary.FirstOrDefault(x => x.Key == ("ExpandedBase" + interval.ToString())).Value.FirstOrDefault(y => y.Value.StyleId == ("button" + interval.ToString())).Value.StyleId.ToString());
            var selection1button = tempDropdownMenu.Content.FindByName<Button>("SelectionButton1");
            var selection2button = tempDropdownMenu.Content.FindByName<Button>("SelectionButton2");
            var placeholderButton = tempDropdownButton.Content.FindByName<Button>("PlaceHolderButton");
            placeholderButton.Clicked += DropDownClicked;
            selection1button.Clicked += SelectionMade;
            selection2button.Clicked += SelectionMade;
            placeholderButton.Text = settings.GetType().GetProperty(dynamicSetting).GetValue(settings).ToString();
            List<String> tempSelectionPossibilities = SelectionPossibilities.FirstOrDefault(x => x.Key == dynamicSetting).Value;
            Button selection3button = null;
            if (interval == 3 || interval == 5)
            {
                selection3button = tempDropdownMenu.selectionControlsDictionary.FirstOrDefault(z => z.Key == "ExtendedSelectionButton1").Value.Content.FindByName<Button>("ExtendedSelectionButton1");
                selection3button.Clicked += SelectionMade;
                ControlsIDDictionary.Add(("selection3button" + interval.ToString()), selection3button.Id);
            }
            int count = 1;
            foreach (string item in tempSelectionPossibilities)
            {
                if (item == placeholderButton.Text)
                {
                    continue;
                }
                Console.WriteLine(item);
                if ((interval == 3 || interval == 5 )&& count == 3)
                {
                    selection3button.Text = item;
                    continue;
                }
                else
                {
                    tempDropdownMenu.Content.FindByName<Button>(("SelectionButton" + count.ToString())).Text = item;
                }
                count++;
            }
            topGrid.Children.Add(tempDropdownMenu, 0, 0);
            topGrid.Children.Add(tempDropdownButton, 0, 0);
            ControlsIDDictionary.Add(("base" + interval.ToString()), tempDropdownMenu.Id);
            ControlsIDDictionary.Add("dropdownButton" + interval.ToString(), placeholderButton.Id);
            ControlsIDDictionary.Add(("selection1button" + interval.ToString()), selection1button.Id);
            ControlsIDDictionary.Add(("selection2button" + interval.ToString()), selection2button.Id);
            Content.FindByName<Button>("FakeButton" + interval.ToString()).Text = "";
            Content.FindByName<Image>("Fake" + interval.ToString()).IsVisible = false;
        }
        private void SelectionMade(object sender, EventArgs args)
        {
            Button btn = (Button)sender;
            int interval = 0;
            string newSetting = btn.Text;
            foreach (KeyValuePair<String, Guid> current in ControlsIDDictionary)
            {
                if (current.Key.Contains("selection") == false)
                {
                    continue;
                }
                string c = current.Key.Replace("selection", "");
                c = c.Replace("button", "");
                char temp = c[1];
                int intervalString = Int32.Parse(temp.ToString());
                if (current.Value != btn.Id)
                {
                    continue;
                }
                else
                {
                    interval = intervalString;
                    break;
                }
            }
            switch (interval)
            {
                case 1:
                    settings.GetType().GetProperty("questions").SetValue(settings, Int32.Parse(newSetting));
                    Console.WriteLine(settings.questions);
                    break;
                case 2:
                    settings.GetType().GetProperty("timelimit").SetValue(settings, Int32.Parse(newSetting));
                    Console.WriteLine(settings.timelimit);
                    break;
                case 3:
                    settings.GetType().GetProperty("clef").SetValue(settings, Enum.Parse(typeof(Classes.Clef), newSetting));
                    Console.WriteLine(settings.clef);
                    break;
                case 5:
                    settings.GetType().GetProperty("accidentals").SetValue(settings, Enum.Parse(typeof(Classes.Accidentals), newSetting));
                    Console.WriteLine(settings.accidentals);
                    break;
                default:
                    return;
            }
            var placeholderButton = DropdownControlsDictionary.FirstOrDefault(x => x.Key == ("ExpandedBase" + interval.ToString())).Value.FirstOrDefault(y => y.Value.StyleId == ("button" + interval.ToString())).Value.Content.FindByName<Button>("PlaceHolderButton");
            btn.Text = placeholderButton.Text;
            placeholderButton.Text = newSetting;
            settings.SaveSettings();
            DropDownClicked(placeholderButton, args);
        }
    }
}