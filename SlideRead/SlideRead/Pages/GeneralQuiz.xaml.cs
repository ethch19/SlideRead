using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SlideRead.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GeneralQuiz : ContentPage
    {
        Classes.Settings settings = new Classes.Settings();
        Classes.KeySignature keySignature = new Classes.KeySignature();
        Dictionary<string, Classes.IClef> clefConfig = new Dictionary<string, Classes.IClef>()
        {
            {"Bass", new Classes.BassClef()},
            {"Tenor", new Classes.TenorClef()},
            {"Treble", new Classes.TrebleClef()}
        };
        Dictionary<string, Guid> AddedViews = new Dictionary<string, Guid>();
        List<string> CScale = new List<string>(); //Trimmed C scale
        List<string> ScaleInKey = new List<string>(); //Trimmed Scale In Key
        Timer timer;
        string answerNote = String.Empty;
        int score = 0;
        int answeredQuestions = 0;
        public GeneralQuiz()
        {
            InitializeComponent();
            ExpandTrimmedScale();
            TuneScaleInKey();
            DisplayKeySignature();
            AccidentalAddtion();
            GetNewQuestion();
        }
        protected override void OnAppearing()//Fade animation on start
        {
            base.OnAppearing();
            topGrid.Opacity = 0;
            topGrid.FadeTo(1, 350);
        }
        private void ExpandTrimmedScale()//Expand C scale into 7 octaves and shorten according to selected clef
        {
            Random random = new Random();
            string iClef = settings.clef.ToString();
            if (iClef == "Mixed")
            {
                iClef = new string[] { "Treble", "Bass", "Tenor" }[random.Next(0, 2)];
            }
            Console.WriteLine(iClef);
            Clef.Source = ImageSource.FromFile(iClef + "Clef.png");
            if (iClef == "Treble")
            {
                Clef.Scale = 0.5;
            }
            else if (iClef == "Bass")
            {
                Clef.Scale = 0.3;
            }
            else if (iClef == "Tenor")
            {
                Clef.TranslationY = -12;
                Clef.Scale = 0.32;
            }
            Classes.IClef clef = clefConfig.First(x => x.Key == settings.clef.ToString()).Value;
            for (int i = 1; i <= keySignature.OctaveNum; i++)
            {
                foreach (string note in keySignature.CMajorOctave)
                {
                    CScale.Add(note + i.ToString());
                }
            }
            int startIndex = CScale.IndexOf(clef.MaxRange[0]);
            int endIndex = CScale.IndexOf(clef.MaxRange[1]);
            var subScale = CScale.GetRange(startIndex, endIndex - startIndex + 1);
            CScale = new List<string>(subScale);
            string s = string.Join(", ", CScale);
            Console.WriteLine($"Trimmed C Scale: {s}");
        }
        private void TuneScaleInKey()//From C scale, make a scale in key of choice
        {
            if (settings.keyFlag == Classes.Key.Neutral)
            {
                ScaleInKey = new List<string>(CScale);
                return;
            }
            foreach (string note in CScale)
            {
                string up = note[0] + "#";
                string down = note[0] + "b";
                //Console.WriteLine($"Up: {up}, Down: {down}");
                List<string> OrderOfKey = (List<string>)keySignature.GetType().GetProperty("OrderOf" + settings.keyFlag.ToString()).GetValue(keySignature);
                bool found = false;
                for (int i = 0; i < settings.numOfKey; i++)
                {
                    string accidental = OrderOfKey[i];
                    if (accidental.Equals(up))
                    {
                        ScaleInKey.Add(up + note[1]);
                        found = true;
                        break;
                    }
                    else if (accidental.Equals(down))
                    {
                        ScaleInKey.Add(down + note[1]);
                        found = true;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
                if (!found)
                {
                    ScaleInKey.Add(note);
                }
            }
            string s = string.Join(", ", ScaleInKey);
            Console.WriteLine($"Scale In Key: {s}");
        }
        private async Task DisplayKeySignature()//Set key signatures (flats/sharps)
        {
            //Key Signature
            int xThreshold = new int[] { 0, 10, 20, 30, 40, 50, 60, 0 }[7 - settings.numOfKey];
            Note.TranslationX = new int[] { 110, 105, 100, 90, 85, 80, 75, 50 }[7 - settings.numOfKey];
            Clef.TranslationX = new int[] { -110, -100, -90, -80, -75, -70, -70, -70 }[7 - settings.numOfKey];
            for (int i = 0; i < settings.numOfKey; i++)
            {
                List<string> OrderOfKey = (List<string>)keySignature.GetType().GetProperty("OrderOf" + settings.keyFlag).GetValue(keySignature);
                Classes.IClef clef = clefConfig.First(x => x.Key == settings.clef.ToString()).Value;
                List<string> MinRange = (List<string>)clef.GetType().GetProperty("MinRange" + settings.keyFlag).GetValue(clef);
                string currentStep = clef.MidNote.ToString();
                for (int j = CScale.IndexOf(MinRange[0]); j <= CScale.IndexOf(MinRange[1]); j++)
                {
                    string checkNote = OrderOfKey[i][0].ToString() + j;
                    int orderIndex = CScale.IndexOf(checkNote);
                    if (orderIndex < 0 || orderIndex < CScale.IndexOf(MinRange[0]))
                    {
                        continue;
                    }
                    int MiddleIndex = CScale.IndexOf(currentStep);
                    int y = 1;
                    Console.WriteLine($"{OrderOfKey[i][0]} Index: {orderIndex}, Middle Line ({currentStep}) Index: {MiddleIndex}");
                    if (orderIndex < MiddleIndex) //Move down
                    {
                        Console.WriteLine("Moving down");
                        for (int q = MiddleIndex - 1; q >= 0; q--)
                        {
                            if (CScale[q] == checkNote)
                            {
                                y = -10 + (12 * y);
                                Console.WriteLine(OrderOfKey[i] + " has " + "y= " + y.ToString());
                                break;
                            }
                            else
                            {
                                y++;
                                currentStep = CScale[q];
                            }
                        }
                    }
                    else if (orderIndex > MiddleIndex) //Move up
                    {
                        Console.WriteLine("Moving up");
                        for (int q = MiddleIndex + 1; q < CScale.Count; q++)
                        {
                            if (CScale[q] == checkNote)
                            {
                                y = -10 - (12 * y);
                                Console.WriteLine(OrderOfKey[i] + " has " + "y= " + y.ToString());
                                break;
                            }
                            else
                            {
                                y++;
                                currentStep = CScale[q];
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("EQUAL TO MID LINE");
                        y = -10;
                    }
                    Image image = new Image()
                    {
                        Source = ImageSource.FromFile(settings.keyFlag + ".png"),
                        Aspect = Aspect.AspectFit,
                        Scale = 0.16,
                        TranslationX = (19 * i - 55) + xThreshold,
                        TranslationY = y,
                        StyleId = settings.keyFlag + "_" + i.ToString()
                    };
                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        topGrid.Children.Insert(topGrid.Children.Count - 3, image);
                    });
                    AddedViews.Add(image.StyleId, image.Id);
                    break;
                }
            }
            switch (settings.numOfKey)
            {
                case 2:
                    break;
                case 1:
                    break;
                case 0:
                    break;
                default:
                    Staff.ScaleX = (0.05 * settings.numOfKey) + 0.9;
                    break;
            }
        }
        private (string selection, int selectionCount) GetRandomNote()//Get a random note as the answer
        {
            //Get random note and slide pos
            Random random = new Random();
            int selectionCount = random.Next(0, ScaleInKey.Count - 1);
            string selection = ScaleInKey[selectionCount];
            answerNote = selection;
            return (selection, selectionCount);
        }
        private async Task<(int steps, int StartIndex, string CScaleSelection)> DisplayNote(string selection)//Move note to correct position
        {
            //Display note on screen
            Classes.IClef clef = clefConfig.First(x => x.Key == settings.clef.ToString()).Value;
            int StartIndex = CScale.IndexOf(clef.MidNote); //List[2] is the note of the middle line of the specific clef
            int EndIndex = 0;
            string currentNote = selection;
            if (selection.Length < 3 && CScale.Contains(selection))
            {
                EndIndex = CScale.IndexOf(selection);
                currentNote = selection;
            }
            else if (selection.Length > 2)
            {
                EndIndex = CScale.IndexOf(selection.Remove(1, 1));
                currentNote = selection.Remove(1, 1);
            }
            else
            {
                Console.WriteLine("EndIndex NOT FOUND");
            }
            Console.WriteLine($"StartIndex: {StartIndex}, EndIndex: {EndIndex}");
            int steps = 1;
            if (EndIndex < StartIndex) //Move down
            {
                for (int q = StartIndex - 1; q >= 0; q--)
                {
                    if (CScale[q] != currentNote)
                    {
                        steps++;
                    }
                    else if (CScale[q] == currentNote)
                    {
                        await TranslateNote(steps, false);
                        break;
                    }
                }
            }
            else if (EndIndex > StartIndex) //Move up
            {
                for (int q = StartIndex + 1; q < CScale.Count; q++)
                {
                    if (CScale[q] != currentNote)
                    {
                        steps++;
                    }
                    if (CScale[q] == currentNote)
                    {
                        await TranslateNote(steps, true);
                        break;
                    }
                }
            }
            return (steps, StartIndex, currentNote);
        }
        private async Task LedgerLines(int steps, int StartIndex, string CScaleSelection)//Make ledger lines if needed
        {
            //Control ledger lines
            Console.WriteLine("Step: " + steps.ToString());
            int selectionCount = CScale.IndexOf(CScaleSelection);
            if (steps >= 6)
            {
                if ((steps - 6) % 2 != 0) //Below or above ledger line
                {
                    if (selectionCount > StartIndex)
                    {
                        await Device.InvokeOnMainThreadAsync(() =>
                        {
                            LedgerLine.TranslationY = Note.TranslationY + 12;
                            LedgerLine.TranslationX = Note.TranslationX;
                        });
                    }
                    else
                    {
                        await Device.InvokeOnMainThreadAsync(() =>
                        {
                            LedgerLine.TranslationY = Note.TranslationY - 12;
                            LedgerLine.TranslationX = Note.TranslationX;
                        });
                    }
                }
                else //On ledger line
                {
                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        LedgerLine.TranslationY = Note.TranslationY;
                        LedgerLine.TranslationX = Note.TranslationX;
                    });
                }
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    LedgerLine.IsVisible = true;
                });
                if ((steps - 6) % 2 == 0 && steps > 7)
                {
                    for (int i = 1; i < ((steps - 6) / 2) + 1; i++)
                    {
                        int temp1 = 0;
                        if (selectionCount < StartIndex)
                        {
                            temp1 = (steps - (2 * i)) * 12;
                        }
                        else
                        {
                            temp1 = (steps - (2 * i)) * -12;
                        }
                        Image image = new Image()
                        {
                            Source = ImageSource.FromFile("LedgerLine"),
                            Scale = 0.15,
                            Aspect = Aspect.AspectFit,
                            TranslationX = Note.TranslationX,
                            TranslationY = temp1,
                            StyleId = "LedgerLine" + i.ToString()
                        };
                        await Device.InvokeOnMainThreadAsync(() =>
                        {
                            topGrid.Children.Insert(topGrid.Children.Count - 2, image);
                        });
                        AddedViews.Add(image.StyleId, topGrid.Children.First(x => x.StyleId == "LedgerLine" + i.ToString()).Id);
                    }
                }
            }
            else
            {
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    LedgerLine.IsVisible = false;
                });
            }
        }
        private async Task<string> AccidentalAddtion()
        {
            Random random = new Random();
            string AccidentalSettings = settings.accidentals.ToString();
            if (random.Next(0, 1) != 0 || AccidentalSettings == "None")
            {
                return null;
            }
            switch (AccidentalSettings)
            {
                case "Mixed":
                    AccidentalSettings = (random.Next(0, 1) == 0) ? "Flat" : "Sharp";
                    break;
                default:
                    break;
            }
            if (answerNote == null)
            {
                return null;
            }
            List<string> cache1 = keySignature.ConvertSharpFlat(ScaleInKey);
            return null;
        }
        private async Task DisplayAnswers()//Set answers on buttons
        {
            //Set answers
            Random random = new Random();
            List<string> answers = new List<string>();
            List<string> wrongAnswers = new List<string>(ScaleInKey);
            answers.Add(answerNote);
            wrongAnswers.Remove(answerNote);
            for (int i = 0; i < 3; i++)
            {
                string selected = wrongAnswers[random.Next(0, wrongAnswers.Count - 1)];
                answers.Add(selected);
                wrongAnswers.Remove(selected);
            }
            for (int i = 0; i < 4; i++)
            {
                string selected = answers[random.Next(0, answers.Count - 1)];
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    ButtonStackLayout.FindByName<Button>("SelectionBtn" + (i + 1).ToString()).Text = selected;
                });
                answers.Remove(selected);
            }
        }
        private async Task ChangeProgress()//Change Progress bar
        {
            answeredQuestions++;
            await Device.InvokeOnMainThreadAsync(() =>
            {
                ProgressBar.ProgressTo((double)answeredQuestions / settings.questions, 800, Easing.SinOut);
            });
        }
        private async void GetNewQuestion()//Request a new question
        {
            //Timer Config
            if (timer == null)
            {
                timer = new Timer(settings.timelimit * 1000);
                timer.Elapsed += HandleTimerElapsed;
                timer.AutoReset = false;
            }
            else
            {
                timer.Stop();
            }

            //Clears AddedViews except key signatures
            foreach (KeyValuePair<string, Guid> item in AddedViews.ToList())
            {
                if (item.Key.Split('_')[0] != settings.keyFlag.ToString())
                {
                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        topGrid.Children.Remove(topGrid.Children.First(x => x.Id == item.Value));
                    });
                    AddedViews.Remove(item.Key);
                }
            }

            //MainFlow
            var list1 = GetRandomNote();
            var list2 = await DisplayNote(list1.selection);
            await LedgerLines(list2.steps, list2.StartIndex, list2.CScaleSelection);
            await DisplayAnswers();
            await ChangeProgress();

            //Start timer
            timer.Start();
        }
        private async Task TranslateNote(int steps, bool up)//Move the note in terms of steps, 1 step = up/down a line/space
        {
            int actualSteps = steps * 12;
            if (up == true)
            {
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    Note.TranslationY = -actualSteps;
                });
            }
            else
            {
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    Note.TranslationY = actualSteps;
                });
            }
        }
        private void HandleTimerElapsed(object sender, ElapsedEventArgs e)//Question Timed out Event
        {
            SelectionMade("TimedOut");
        }
        private void BtnSelectionMade(object sender, EventArgs args)//SelectionButton Pressed Event
        {
            Button btn = (Button)sender;
            SelectionMade(btn.Text);
        }
        private async void SelectionMade(string text)//Question has been answered event
        {
            if (text == answerNote)
            {
                Console.WriteLine($"CORRECT: {answerNote}");
                score++;
            }
            else
            {
                Console.WriteLine($"INCORRECT: {answerNote}");
            }
            if (answeredQuestions == settings.questions)
            {
                Console.WriteLine($"SCORE: {score}/{settings.questions}");
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    Application.Current.MainPage.Navigation.PopAsync(false);
                });
            }
            GetNewQuestion();
        }
    }
}