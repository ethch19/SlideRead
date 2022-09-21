using SlideRead.Controls;
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
    public partial class MainQuiz : ContentPage
    {
        Classes.Settings settings = new Classes.Settings();
        Classes.KeySignature keySignature = new Classes.KeySignature();
        Dictionary<string, Classes.IClef> clefConfig = new Dictionary<string, Classes.IClef>()
        {
            {"Bass", new Classes.BassClef()},
            {"Tenor", new Classes.TenorClef()}
        };
        Dictionary<string, Guid> AddedViews = new Dictionary<string, Guid>();
        List<string> CScale = new List<string>(); //Trimmed C scale
        List<string> ScaleInKey = new List<string>(); //Trimmed Scale In Key
        List<string> ChromScale = new List<string>();
        Timer timer = new Timer();
        bool Termination = false;
        int answerPos = 0;
        string answerNote = string.Empty;
        string lastAnswer = string.Empty;
        int score = 0;
        int answeredQuestions = 0;
        string accidentalApplied = "None";
        bool debugMode = false;
        public MainQuiz()
        {
            InitializeComponent();
            ExpandTrimmedScale();
            TuneScaleInKey();
            DisplayKeySignature();
            GetNewQuestion();
        }
        protected override void OnAppearing()//Fade animation on start
        {
            base.OnAppearing();
            topGrid.Opacity = 0;
            topGrid.FadeTo(1, 350);
        }
        private List<string> ChromaticScale(List<string> trimmedScale)
        {
            List<string> result = new List<string>(trimmedScale);
            for (int i = 0; i < trimmedScale.Count; i++)
            {
                if (trimmedScale[i][0] == 'E' || trimmedScale[i][0] == 'B') { continue; }
                result.Insert(result.IndexOf(trimmedScale[i]) + 1, trimmedScale[i][0] + "#" + trimmedScale[i][1]);
            }
            string s = string.Join(", ", result);
            Console.WriteLine($"Trimmed Chromatic Scale: {s}");
            return result;
        }
        private void ExpandTrimmedScale()//Expand C scale into 7 octaves and shorten according to selected clef
        {
            Random random = new Random();
            string iClef = settings.clef.ToString();
            if (iClef == "Mixed")
            {
                iClef = new string[] { "Bass", "Tenor" }[random.Next(0, 2)]; //No treble clef for trombone yet
            }
            Console.WriteLine($"Clef chosen: {iClef}");
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
            ChromScale = ChromaticScale(CScale);
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
            int xThreshold = new int[] { 0, 10, 20, 30, 40, 50, 60, 0 }[7 - settings.numOfKey]; //How spaced apart each accidental is
            Note.TranslationX = new int[] { 110, 105, 100, 90, 85, 80, 75, 50 }[7 - settings.numOfKey];
            Clef.TranslationX = new int[] { -110, -100, -90, -80, -75, -70, -70, -70 }[7 - settings.numOfKey];
            for (int i = 0; i < settings.numOfKey; i++)
            {
                List<string> OrderOfKey = (List<string>)keySignature.GetType().GetProperty("OrderOf" + settings.keyFlag).GetValue(keySignature);
                Classes.IClef clef = clefConfig.First(x => x.Key == settings.clef.ToString()).Value;
                List<string> MinRange = (List<string>)clef.GetType().GetProperty("MinRange" + settings.keyFlag).GetValue(clef);
                string currentStep = clef.MidNote.ToString();
                for (int j = int.Parse(MinRange[0][1].ToString()); j <= int.Parse(MinRange[1][1].ToString()); j++)
                {
                    Console.WriteLine(j);
                    string checkNote = OrderOfKey[i][0].ToString() + j;
                    int orderIndex = CScale.IndexOf(checkNote);
                    if (orderIndex < 0 || orderIndex < CScale.IndexOf(MinRange[0]) || orderIndex > CScale.IndexOf(MinRange[1]))
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
                        Scale = (settings.keyFlag == Classes.Key.Flat) ? 0.16 : 0.20,
                        TranslationX = (19 * i - 55) + xThreshold,
                        TranslationY = (settings.keyFlag == Classes.Key.Flat) ? y : y + 10,
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
        private string AccidentalAddtion(int selectionCount)
        {
            Random random = new Random();
            string selection = ScaleInKey[selectionCount];
            string AccidentalSettings = settings.accidentals.ToString();
            if (random.Next(0, 1) != 0 || AccidentalSettings == "None" || selection == "E2" || selection == "Fb2" || selection == "Bb4" || selection == "A#4")
            {
                accidentalApplied = "None";
                return selection;
            }
            if (AccidentalSettings == "Mixed")
            {
                AccidentalSettings = (random.Next(0, 1) == 0) ? "Flat" : "Sharp";
                Console.WriteLine($"Accidental Chosen from Mixed: {AccidentalSettings}");
            }
            int chromCount = 0;
            string newSelection = string.Empty;
            List<string> convChromScale = keySignature.ConvertSharpFlat(ChromScale);
            if (selection.Length == 3)
            {
                if (selection[1] == '#')
                {
                    chromCount = ChromScale.IndexOf(selection);
                    if (AccidentalSettings == "Flat")
                    {
                        newSelection = convChromScale[chromCount - 1];
                    }
                    else
                    {
                        newSelection = ChromScale[chromCount + 1];
                    }
                }
                else
                {
                    chromCount = convChromScale.IndexOf(selection);
                    if (AccidentalSettings == "Flat")
                    {
                        newSelection = convChromScale[chromCount - 1];
                    }
                    else
                    {
                        newSelection = ChromScale[chromCount + 1];
                    }
                }
            }
            else
            {
                if (AccidentalSettings == "Flat")
                {
                    if (selection[0] == 'F') { newSelection = "E" + selection[1]; }
                    else if (selection[0] == 'C') { newSelection = "B" + (int.Parse(selection[1].ToString()) - 1); }
                    else { newSelection = selection.Insert(1, "b"); }
                }
                else
                {
                    if (selection[0] == 'E') { newSelection = "F" + selection[1]; }
                    else if (selection[0] == 'B') { newSelection = "C" + (int.Parse(selection[1].ToString()) + 1); }
                    else { newSelection = selection.Insert(1, "#"); }
                }
            }
            accidentalApplied = AccidentalSettings;
            Console.WriteLine($"Before {accidentalApplied}: {selection}, After {accidentalApplied}: {newSelection}");
            return newSelection;
        }
        private string AccidentalAddtion(int selectionCount, string accidental)
        {
            string selection = ScaleInKey[selectionCount];
            if (selection == "E2" || selection == "Fb2" || selection == "Bb4" || selection == "A#4" || accidental == "None")
            {
                accidentalApplied = "None";
                return selection;
            }
            int chromCount = 0;
            string newSelection = string.Empty;
            List<string> convChromScale = keySignature.ConvertSharpFlat(ChromScale);
            if (selection.Length == 3)
            {
                if (selection[1] == '#')
                {
                    chromCount = ChromScale.IndexOf(selection);
                    if (accidental == "Flat")
                    {
                        newSelection = convChromScale[chromCount - 1];
                    }
                    else
                    {
                        newSelection = ChromScale[chromCount + 1];
                    }
                }
                else
                {
                    chromCount = convChromScale.IndexOf(selection);
                    if (accidental == "Flat")
                    {
                        newSelection = convChromScale[chromCount - 1];
                    }
                    else
                    {
                        newSelection = ChromScale[chromCount + 1];
                    }
                }
            }
            else
            {
                if (accidental == "Flat")
                {
                    if (selection[0] == 'F') { newSelection = "E" + selection[1]; }
                    else if (selection[0] == 'C') { newSelection = "B" + (int.Parse(selection[1].ToString()) - 1); }
                    else { newSelection = selection.Insert(1, "b"); }
                }
                else
                {
                    if (selection[0] == 'E') { newSelection = "F" + selection[1]; }
                    else if (selection[0] == 'B') { newSelection = "C" + (int.Parse(selection[1].ToString()) + 1); }
                    else { newSelection = selection.Insert(1, "#"); }
                }
            }
            accidentalApplied = accidental;
            Console.WriteLine($"Before {accidentalApplied}: {selection}, After {accidentalApplied}: {newSelection}");
            return newSelection;
        }
        private (string selection, string displayNote, int selectionCount, int slidePos) GetRandomNote()//Get a random note as the answer
        {
            //Get random note and slide pos
            Random random = new Random();
            int selectionCount = random.Next(0, ScaleInKey.Count - 1);
            string selection = ScaleInKey[selectionCount];
            if (answeredQuestions != 0)
            {
                while (selection == lastAnswer)
                {
                    selectionCount = random.Next(0, ScaleInKey.Count - 1);
                    selection = ScaleInKey[selectionCount];
                }
            }
            string displayNote = selection;
            selection = AccidentalAddtion(selectionCount);
            int chromCount = 0;
            if (selection.Length == 3)
            {
                if (selection[1] == '#')
                {
                    chromCount = ChromScale.IndexOf(selection);
                }
                else
                {
                    List<string> convChromScale = keySignature.ConvertSharpFlat(ChromScale);
                    chromCount = convChromScale.IndexOf(selection);
                }
            }
            else
            {
                chromCount = ChromScale.IndexOf(selection);
            }
            int tempSelectionCount = chromCount;
            int slidePos = 0;
            List<int> MaxPos = new List<int>() { 7, 7, 5, 4, 3, 3, 3 }; //Always start with E2 as E2 is 7th position. Counts down
            for (int i = 0; i < MaxPos.Count; i++)
            {
                if (i == 5)
                {
                    switch (tempSelectionCount)
                    {
                        case 2:
                            slidePos = 3;
                            break;
                        case 3:
                            slidePos = 2;
                            break;
                        case 4:
                            slidePos = 1;
                            break;
                        default:
                            slidePos = MaxPos[i] - tempSelectionCount;
                            break;
                    }
                    break;
                }
                if (tempSelectionCount + 1 <= MaxPos[i])
                {
                    slidePos = MaxPos[i] - tempSelectionCount;
                    Console.WriteLine(selection + ": " + slidePos.ToString());
                    break;
                }
                else
                {
                    tempSelectionCount -= MaxPos[i];
                }
            }
            answerPos = slidePos;
            answerNote = selection;
            lastAnswer = answerNote;
            return (selection, displayNote, selectionCount, slidePos);
        }
        private (string selection, string displayNote, int slidePos) GetNote(string selection)//Get the note as the answer
        {
            //Get note and slide pos
            string displayNote = selection;
            List<string> convScaleInKey = keySignature.ConvertSharpFlat(ScaleInKey);
            Console.WriteLine(selection);
            if (!ScaleInKey.Contains(selection))
            {
                Console.WriteLine($"Selection not found in ScaleInKey. Selection: {selection}");
                foreach (string key in ScaleInKey)
                {
                    if (key[0] == selection[0])
                    {
                        Console.WriteLine($"Key: {key}, Selection: {selection}");
                        if (key.Length > 2)
                        {
                            if (selection.Length == 2 && key[2] == selection[1])
                            {
                                displayNote = key;
                                break;
                            }
                            else if (selection.Length > 2 && key[2] == selection[2])
                            {
                                displayNote = key;
                                break;
                            }
                            break;
                        }
                        else if (key.Length == 2)
                        {
                            if (selection.Length == 2 && key[1] == selection[1])
                            {
                                displayNote = key;
                                break;
                            }
                            else if (selection.Length > 2 && key[1] == selection[2])
                            {
                                displayNote = key;
                                break;
                            }
                        }
                    }
                }
                if (selection.Length > 2 && displayNote.Length == 2)
                {
                    selection = AccidentalAddtion(ScaleInKey.IndexOf(displayNote), ((selection[1] == '#') ? "Sharp" : "Flat"));
                }
                else if (selection.Length == 2 && displayNote.Length > 2) //DisplayNote: Bb2, Selection: B2 (Neutral)
                {
                    Console.WriteLine("Currently doesn't support double-flats or double-sharps");
                    //selection = AccidentalAddtion(ScaleInKey.IndexOf(displayNote), ((displayNote[1] == '#') ? "Sharp" : "Flat"));
                }
                else //DisplayNote: Eb2, Selection: E3 (Neutral)
                {
                    Console.WriteLine("Some stupid exception idk");
                }
            }
            else
            {
                accidentalApplied = "None";
            }
            int chromCount = 0;
            if (selection.Length == 3)
            {
                if (selection[1] == '#')
                {
                    chromCount = ChromScale.IndexOf(selection);
                }
                else
                {
                    List<string> convChromScale = keySignature.ConvertSharpFlat(ChromScale);
                    chromCount = convChromScale.IndexOf(selection);
                }
            }
            else
            {
                chromCount = ChromScale.IndexOf(selection);
            }
            int slidePos = 0;
            List<int> MaxPos = new List<int>() { 7, 7, 5, 4, 3, 3, 3 }; //Always start with E2 as E2 is 7th position. Counts down
            for (int i = 0; i < MaxPos.Count; i++)
            {
                if (i == 5)
                {
                    switch (chromCount)
                    {
                        case 2:
                            slidePos = 3;
                            break;
                        case 3:
                            slidePos = 2;
                            break;
                        case 4:
                            slidePos = 1;
                            break;
                        default:
                            slidePos = MaxPos[i] - chromCount;
                            break;
                    }
                    break;
                }
                if (chromCount + 1 <= MaxPos[i])
                {
                    slidePos = MaxPos[i] - chromCount;
                    Console.WriteLine(selection + ": " + slidePos.ToString());
                    break;
                }
                else
                {
                    chromCount -= MaxPos[i];
                }
            }
            answerPos = slidePos;
            answerNote = selection;
            lastAnswer = answerNote;
            return (selection, displayNote, slidePos);
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
            else
            {
                await TranslateNote(0, false);
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
                else if ((steps - 7) % 2 == 0 && steps > 7)
                {
                    for (int i = 1; i < Math.Ceiling((double)(steps - 6) / 2) + 1; i++)
                    {
                        int yTranslation = 0;
                        if (selectionCount < StartIndex)
                        {
                            yTranslation = (steps - (2 * i) + 1) * 12;
                        }
                        else
                        {
                            yTranslation = (steps - (2 * i) + 1) * -12;
                        }

                        Image image = new Image()
                        {
                            Source = ImageSource.FromFile("LedgerLine"),
                            Scale = 0.15,
                            Aspect = Aspect.AspectFit,
                            TranslationX = Note.TranslationX,
                            TranslationY = yTranslation,
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
        private async Task DisplayAnswers(int slidePos)//Set answers on buttons
        {
            //Set answers
            Random random = new Random();
            List<string> answers = new List<string>();
            List<string> wrongAnswers = new List<string>() { "1", "2", "3", "4", "5", "6", "7" };
            answers.Add(slidePos.ToString());
            wrongAnswers.Remove(slidePos.ToString());
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
            await Device.InvokeOnMainThreadAsync(() => //Dev answer display
            {
                AnswerDisplay.Text = $"Note: {answerNote}, Slide Pos: {answerPos}";
            });
        }
        private async Task DisplayAccidental()
        {
            if (accidentalApplied == "None") { return; }
            int yTranslation = (accidentalApplied == "Sharp") ? 0 : 10;
            Image image = new Image()
            {
                Source = ImageSource.FromFile(accidentalApplied + ".png"),
                Aspect = Aspect.AspectFit,
                Scale = 0.16,
                TranslationX = Note.TranslationX - 25,
                TranslationY = Note.TranslationY - yTranslation,
                StyleId = accidentalApplied + "Acci"
            };
            await Device.InvokeOnMainThreadAsync(() =>
            {
                topGrid.Children.Insert(topGrid.Children.Count - 3, image);
            });
            AddedViews.Add(image.StyleId, image.Id);
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
            if (Termination) return;
            //Timer Config
            timer = new Timer(settings.timelimit * 1000);
            timer.Elapsed += HandleTimerElapsed;
            timer.AutoReset = false;

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
            Console.WriteLine($"displayNote: {list1.displayNote}, selection: {list1.selection}");
            var list2 = await DisplayNote(list1.displayNote);
            await DisplayAccidental();
            await LedgerLines(list2.steps, list2.StartIndex, list2.CScaleSelection);
            await DisplayAnswers(answerPos);
            await ChangeProgress();

            for (int i = 1; i < 4; i++)
            {
                ButtonStackLayout.FindByName<CustomBtn>("SelectionBtn" + i.ToString()).IsEnabled = true;
            }
            //Start timer
            timer.Start();
        }
        private async void GetNewQuestion(string selection)//Request a new question
        {
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
            var list1 = GetNote(selection);
            Console.WriteLine($"displayNote: {list1.displayNote}, selection: {list1.selection}");
            var list2 = await DisplayNote(list1.displayNote);
            await DisplayAccidental();
            await LedgerLines(list2.steps, list2.StartIndex, list2.CScaleSelection);
            await DisplayAnswers(answerPos);
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
            if (debugMode == true) { return; }
            SelectionMade("TimedOut", "n/a");
        }
        private void BtnSelectionMade(object sender, EventArgs args)//SelectionButton Pressed Event
        {
            Button btn = (Button)sender;
            for (int i = 1; i < 4; i++)
            {
                ButtonStackLayout.FindByName<CustomBtn>("SelectionBtn" + i.ToString()).IsEnabled = false;
            }
            Console.WriteLine(SelectionBtn1.IsEnabled);
            SelectionMade(btn.Text, "n/a");
        }
        private async void SelectionMade(string text, string selection)//Question has been answered event
        {
            timer.Stop();
            if (text == answerPos.ToString())
            {
                Console.WriteLine($"CORRECT: {answerNote} slide position = {answerPos}");
                if (!debugMode) { score++; }
            }
            else
            {
                Console.WriteLine($"INCORRECT: {answerNote} slide position = {answerPos}");
            }
            if (answeredQuestions == settings.questions && !debugMode)
            {
                Termination = true;
                Console.WriteLine($"SCORE: {score}/{settings.questions}");
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    Navigation.PushAsync(new ScorePage(score, settings.questions));
                });
            }
            if (!debugMode)
            {
                GetNewQuestion();
            }
            else
            {
                GetNewQuestion(selection);
            }
        }
        private void DebugMode(object sender, EventArgs args)
        {
            if (debugMode == false)
            {
                debugMode = true;
                UpButton.IsVisible = true;
                DownButton.IsVisible = true;
                AnswerDisplay.IsVisible = true;
                DebugButton.Text = "HIDE";
                Classes.IClef clef = clefConfig.First(x => x.Key == settings.clef.ToString()).Value;
                SelectionMade("DebugMode", clef.MidNote);
            }
            else
            {
                debugMode = false;
                UpButton.IsVisible = false;
                DownButton.IsVisible = false;
                AnswerDisplay.IsVisible = false;
                DebugButton.Text = "DEBUG";
                SelectionMade("QuizMode", "n/a");
            }
        }
        private void NoteDebug(object sender, EventArgs args)
        {
            Controls.CustomBtn button = (Controls.CustomBtn)sender;
            if (button.Text == "˅") //Down
            {
                if (answerNote == "E2" || answerNote == "Fb2")
                {
                    return;
                }
            }
            else
            {
                if (answerNote == "Bb4" || answerNote == "A#4")
                {
                    return;
                }
            }
            int chromCount = 0;
            string newSelection = string.Empty;
            List<string> convChromScale = keySignature.ConvertSharpFlat(ChromScale);
            if (answerNote.Length == 3)
            {
                if (answerNote[1] == '#')
                {
                    chromCount = ChromScale.IndexOf(answerNote);
                    if (button.Text == "˅") //Down
                    {
                        newSelection = convChromScale[chromCount - 1];
                    }
                    else
                    {
                        newSelection = ChromScale[chromCount + 1];
                    }
                }
                else
                {
                    chromCount = convChromScale.IndexOf(answerNote);
                    if (button.Text == "˅") //Down
                    {
                        newSelection = convChromScale[chromCount - 1];
                    }
                    else
                    {
                        newSelection = ChromScale[chromCount + 1];
                    }
                }
            }
            else
            {
                if (button.Text == "˅") //Down
                {
                    if (answerNote[0] == 'F') { newSelection = "E" + answerNote[1]; }
                    else if (answerNote[0] == 'C') { newSelection = "B" + (int.Parse(answerNote[1].ToString()) - 1); }
                    else { newSelection = answerNote.Insert(1, "b"); }
                }
                else
                {
                    if (answerNote[0] == 'E') { newSelection = "F" + answerNote[1]; }
                    else if (answerNote[0] == 'B') { newSelection = "C" + (int.Parse(answerNote[1].ToString()) + 1); }
                    else { newSelection = answerNote.Insert(1, "#"); }
                }
            }
            Console.WriteLine($"OldNote: {answerNote}, NewNote: {newSelection}");
            SelectionMade("DebugMode", newSelection);
        }
    }
}