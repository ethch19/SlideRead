using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SlideRead.Controls;

namespace SlideRead.Pages
{
    public partial class SettingsPage : ContentPage
    {
        Dictionary<String, bool> settingOptions = new Dictionary<string, bool>()
        {
            { "questions", false},
            { "timelimit", false},
            { "clef", false},
            { "accidentals", false}
        };
        Classes.Settings settings = new Classes.Settings();
        Dictionary<String, List<String>> SelectionPossibilities = new Dictionary<string, List<string>>()
        {
            {"questions", new List<string>(){ "10", "15", "20"} },
            {"timelimit", new List<string>(){ "5", "10", "15"} },
            {"clef", new List<string>(){ "Treble", "Bass", "Tenor", "Mixed"} },
            {"accidentals", new List<string>(){ "Sharp", "Flat", "Mixed", "None"} },
        };
        readonly Dictionary<String, List<String>> keyDictionary = new Dictionary<string, List<String>>()
        {
            { "C/Am", new List<string>(){ "Neutral", "0"} },
            { "G/Em", new List<string>(){ "Sharp", "1"} },
            { "D/Bm", new List<string>(){ "Sharp", "2"} },
            { "A/F<sup><small>♯</small></sup>m", new List<string>(){ "Sharp", "3"} },
            { "E/C<sup><small>♯</small></sup>m", new List<string>(){ "Sharp", "4"} },
            { "B/G<sup><small>♯</small></sup>m", new List<string>(){ "Sharp", "5"} },
            { "F<sup><small>♯</small></sup>/D<sup><small>♯</small></sup>m", new List<string>(){ "Sharp", "6"} },
            { "C<sup><small>♯</small></sup>/A<sup><small>♯</small></sup>m", new List<string>(){ "Sharp", "7"} },
            { "F/Dm", new List<string>(){ "Flat", "1"} },
            { "B<sup><small>♭</small></sup>/Gm", new List<string>(){ "Flat", "2"} },
            { "E<sup><small>♭</small></sup>/Cm", new List<string>(){ "Flat", "3"} },
            { "A<sup><small>♭</small></sup>/Fm", new List<string>(){ "Flat", "4"} },
            { "D<sup><small>♭</small></sup>/B<sup><small>♭</small></sup>m", new List<string>(){ "Flat", "5"} },
            { "G<sup><small>♭</small></sup>/E<sup><small>♭</small></sup>m", new List<string>(){ "Flat", "6"} },
            { "C<sup><small>♭</small></sup>/A<sup><small>♭</small></sup>m", new List<string>(){ "Flat", "7"} }
        };
        public SettingsPage()
        {
            InitializeComponent();
            DpButton1.Text = settings.questions.ToString();
            DpButton2.Text = settings.timelimit.ToString();
            DpButton3.Text = settings.clef.ToString();
            DpButton4.Text = settings.accidentals.ToString();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            absLayout.Opacity = 0;
            absLayout.FadeTo(1, 350);
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
            CustomStackLayout stacklayout = new CustomStackLayout
            {
                HeightRequest = 650,
                WidthRequest = 300,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                SLCornerRadius = 50,
                SLBackgroundColor = Color.FromHex("A386BC"),
                Orientation = StackOrientation.Vertical,
                StyleId = "scrollList"
            };
            CustomLabel mainLabel = new CustomLabel
            {
                HeightRequest = 70,
                WidthRequest = 300,
                Text = settings.displaykey.ToString(),
                TextType = TextType.Html,
                FontSize = 25,
                TextColor = Color.FromHex("A386BC"),
                FontFamily = "Roboto-Medium",
                LabelBackgroundColor = Color.FromHex("F6EBFF"),
                LabelCornerRadius = 50,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(7, 7, 7, 0),
                StyleId = "mainLabel"
            };
            Frame mainFrame = new Frame
            {
                CornerRadius = 18,
                BackgroundColor = Color.FromHex("F5E9FF"),
                Margin = new Thickness(7, 3, 7, 7),
                Padding = new Thickness(8, 8, 8, 8),
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            ScrollView scrollview = new ScrollView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            Grid grid = new Grid
            {
                ColumnDefinitions = 
                { 
                    new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)},
                    new ColumnDefinition {Width = new GridLength(4, GridUnitType.Star)}
                },
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            int index = 0;
            foreach (KeyValuePair<string, List<string>> keyValuePair in keyDictionary)
            {
                string key = string.Empty;
                switch (keyValuePair.Value[0])
                {
                    case "Flat":
                        key = "<small><sup>♭<sup><small>";
                        break;
                    case "Sharp":
                        key = "<small><sup>♯<sup><small>";
                        break;
                    case "Neutral":
                        key = "";
                        break;
                }
                CustomLabel customLabel = new CustomLabel
                {
                    Text = keyValuePair.Value[1] + key,
                    TextType = TextType.Html,
                    LabelBackgroundColor = Color.FromHex("8B719E"),
                    LabelCornerRadius = 30,
                    FontSize = 18,
                    TextColor = Color.FromHex("FFFFFF"),
                    FontFamily = "Roboto-Medium",
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center
                };
                CustomBtn customBtn = new CustomBtn
                {
                    Text = keyValuePair.Key,
                    ButtonHtml = true,
                    ButtonBackgroundColor = Color.FromHex("B392CC"),
                    ButtonCornerRadius = 30,
                    FontSize = 18,
                    TextColor = Color.FromHex("FFFFFF"),
                    FontFamily = "Roboto-Medium"
                };
                customBtn.Clicked += KeyChange;
                grid.RowDefinitions.Add(new RowDefinition() { Height = 45 });
                grid.Children.Add(customLabel, 0, index);
                grid.Children.Add(customBtn, 1, index);
                index++;
            }
            scrollview.Content = grid;
            mainFrame.Content = scrollview;
            stacklayout.Children.Add(mainLabel);
            stacklayout.Children.Add(mainFrame);
            absLayout.RaiseChild(DarkenedLayer);
            absLayout.RaiseChild(scrollListExitBtn);
            absLayout.Children.Add(stacklayout, new Rectangle(new Point(0.5, 0.3), new Size(1, 1)), AbsoluteLayoutFlags.All);
            btn.IsEnabled = true;
        }
        private void KeyChange(object sender, EventArgs args)
        {
            CustomBtn btn = (CustomBtn)sender;
            CustomStackLayout scrollList = (CustomStackLayout)absLayout.Children.First(p => p.StyleId == "scrollList");
            CustomLabel mainLabel = (CustomLabel)scrollList.Children.First(q => q.StyleId == "mainLabel");
            mainLabel.Text = btn.Text;
        }
        private void CloseScrollList(object sender, EventArgs args)
        {
            ImageButton btn = (ImageButton)sender;
            if (btn.IsEnabled == false) return;
            btn.IsEnabled = false;
            CustomStackLayout scrollList = (CustomStackLayout)absLayout.Children.First(p => p.StyleId == "scrollList");
            CustomLabel mainLabel = (CustomLabel)scrollList.Children.First(q => q.StyleId == "mainLabel");
            settings.GetType().GetProperty("displaykey").SetValue(settings, mainLabel.Text);
            List<string> tempInfo = keyDictionary.First(q => q.Key == mainLabel.Text).Value;
            settings.GetType().GetProperty("keyFlag").SetValue(settings, Enum.Parse(typeof(Classes.Key), tempInfo[0]));
            settings.GetType().GetProperty("numOfKey").SetValue(settings, Int32.Parse(tempInfo[1]));
            settings.SaveSettings();
            absLayout.Children.Remove(scrollList);
            DarkenedLayer.IsVisible = false;
            btn.IsVisible = false;
            btn.IsEnabled = true;
        }
        private void DropDownClicked(object sender, EventArgs args)
        {
            CustomBtn btn = (CustomBtn)sender;
            if (btn.IsEnabled == false)
            {
                return;
            }
            btn.IsEnabled = false;
            int interval = 0;
            // Get the pressed button's state and setting
            for (int i = 1; i <= settingOptions.Count; i++)
            {
                if (btn.Id == absLayout.FindByName<CustomBtn>("DpButton" + i).Id)
                {
                    interval = i - 1;
                }
            }
            // If there is already a dropdown menu, deletes it
            if (settingOptions.Values.ToList()[interval] == true)
            {
                var view = absLayout.Children.First(x => x.StyleId == settingOptions.Keys.ToList()[interval] + "Dp");
                absLayout.Children.Remove(view);
                int count1 = 0;
                string key1 = string.Empty;
                foreach (KeyValuePair<string, bool> keyValuePair in settingOptions)
                {
                    if (count1 == interval)
                    {
                        key1 = keyValuePair.Key;
                        break;
                    }
                    count1++;
                }
                settingOptions[key1] = false;
                btn.IsEnabled = true;
                return;
            }
            // Create a dropdown menu and button IF there isn't one
            Point parentPos = new Point(btn.X, btn.Y);
            var parent = btn.Parent as VisualElement;
            while (parent != null)
            {
                parentPos.X += parent.X;
                parentPos.Y += parent.Y;
                parent = parent.Parent as VisualElement;
            }
            Console.WriteLine(parentPos.X.ToString() + ", " + parentPos.Y.ToString());
            CustomStackLayout stacklayoutTemp = new CustomStackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(3, 20, 3, 4),
                SLBackgroundColor = Color.FromHex("#9369A7"),
                SLCornerRadius = new CornerRadius(0, 0, 45, 45),
                StyleId = settingOptions.Keys.ToList()[interval] + "Dp"
            };
            List<String> tempSelectionPossibilities = SelectionPossibilities.First(x => x.Key == settingOptions.Keys.ToList()[interval]).Value.Where(y => y != btn.Text).ToList();
            if (interval == 2 || interval == 3)
            {
                for (int i = 1; i < 4; i++)
                {
                    CustomBtn customBtn = new CustomBtn()
                    {
                        ButtonBackgroundColor = Color.FromHex("#AE85C2"),
                        ButtonBorderWidth = 8,
                        ButtonCornerRadius = 30,
                        HeightRequest = 30,
                        WidthRequest = 95,
                        TextColor = Color.FromHex("#FFFFFF"),
                        FontFamily = "Roboto-Medium"
                    };
                    customBtn.StyleId = "SelectionButton" + i;
                    customBtn.Clicked += SelectionMade;
                    customBtn.Text = tempSelectionPossibilities[i - 1];
                    stacklayoutTemp.Children.Add(customBtn);
                }
            }
            else
            {
                for (int i = 1; i < 3; i++)
                {
                    CustomBtn customBtn = new CustomBtn()
                    {
                        ButtonBackgroundColor = Color.FromHex("#AE85C2"),
                        ButtonBorderWidth = 8,
                        ButtonCornerRadius = 30,
                        HeightRequest = 30,
                        WidthRequest = 95,
                        TextColor = Color.FromHex("#FFFFFF"),
                        FontFamily = "Roboto-Medium"
                    };
                    customBtn.StyleId = "SelectionButton" + i;
                    customBtn.Clicked += SelectionMade;
                    customBtn.Text = tempSelectionPossibilities[i - 1];
                    stacklayoutTemp.Children.Add(customBtn);
                }
            }
            parentPos.Y += 18;
            absLayout.Children.Add(stacklayoutTemp, parentPos);
            absLayout.RaiseChild(btn);
            int count = 0;
            string key = string.Empty;
            foreach (KeyValuePair<string, bool> keyValuePair in settingOptions)
            {
                if (count == interval)
                {
                    key = keyValuePair.Key;
                    break;
                }
                count++;
            }
            settingOptions[key] = true;
            btn.IsEnabled = true;
        }
        private void SelectionMade(object sender, EventArgs args)
        {
            CustomBtn btn = (CustomBtn)sender;
            string newSetting = btn.Text;
            string parentId = btn.Parent.StyleId.Replace("Dp", "");
            int interval = settingOptions.Keys.ToList().IndexOf(parentId);
            interval++;
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
                case 4:
                    settings.GetType().GetProperty("accidentals").SetValue(settings, Enum.Parse(typeof(Classes.Accidentals), newSetting));
                    Console.WriteLine(settings.accidentals);
                    break;
                default:
                    return;
            }
            var parentBtn = absLayout.FindByName<CustomBtn>("DpButton" + interval);
            parentBtn.Text = newSetting;
            settings.SaveSettings();
            DropDownClicked(parentBtn, args);
        }
    }
}