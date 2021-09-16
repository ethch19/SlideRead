using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Timers;
using System.Linq;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SlideRead.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainQuiz : ContentPage
    {
        Classes.Settings settings = new Classes.Settings();
        Classes.TromboneConfig tromboneConfig = new Classes.TromboneConfig();
        Classes.ClefConfig clefConfig = new Classes.ClefConfig();
        Dictionary<string, Guid> AddedViews = new Dictionary<string, Guid>();
        Dictionary<string, List<string>> AllNotes = new Dictionary<string, List<string>>();
        List<string> KeyNotes = new List<string>();
        Timer timer;
        string currentAnswer = String.Empty;
        string AnswerNote = String.Empty;
        int score = 0;
        int answeredQuestions = 0;
        public MainQuiz()
        {
            InitializeComponent();
            Deserialisation();
            AllNotes.Add("Flat", CreateNotes("Flat"));
            AllNotes.Add("Sharp", CreateNotes("Sharp"));
        }
        //Fade animation on start
        protected override void OnAppearing()
        {
            base.OnAppearing();
            GetNewQuestion();
            topGrid.Opacity = 0;
            topGrid.FadeTo(1, 350);
        }
        //Request a new question
        private async void GetNewQuestion()
        {
            Random random = new Random();

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

            //Clef Config
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

            //Clears AddedViews
            foreach (KeyValuePair<string, Guid> item in AddedViews)
            {
                topGrid.Children.Remove(topGrid.Children.First(x => x.Id == item.Value));
            }
            AddedViews.Clear();

            //Key Signature
            string keyFlag = settings.keyFlag.ToString() == "Neutral" ? "Flat" : settings.keyFlag.ToString();
            KeyNotes = GetKeyNotes(keyFlag);
            int xThreshold = new int[] { 0, 10, 20, 30, 40, 50, 60, 0 }[7 - settings.numOfKey];
            Note.TranslationX = new int[] { 110, 105, 100, 90, 85, 80, 75, 50 }[7 - settings.numOfKey];
            Clef.TranslationX = new int[] { -110, -100, -90, -80, -75, -70, -70, -70 }[7 - settings.numOfKey];
            for (int i = 0; i<settings.numOfKey; i++)
            {
                List<string> orderList = (List<string>)clefConfig.GetType().GetProperty("OrderOf" + keyFlag).GetValue(clefConfig);
                List<string> iClefList = (List<string>)clefConfig.GetType().GetProperty(iClef).GetValue(clefConfig);
                int orderIndex = AllNotes[keyFlag].IndexOf(orderList[i]);
                string currentStep = (iClef == "Bass" && keyFlag == "Flat") ? iClefList[iClefList.Count - 2] : iClefList[iClefList.Count - 1];
                int MiddleIndex = AllNotes[keyFlag].IndexOf(currentStep);
                int y = 0;
                Console.WriteLine("Flat/Sharp Index: " + orderIndex.ToString());
                Console.WriteLine("Middle Line Index: " + MiddleIndex.ToString());
                if (orderIndex < MiddleIndex) //Move down
                {
                    Console.WriteLine("Moving down");
                    for (int q = MiddleIndex - 1; q >= 0; q--)
                    {
                        if (AllNotes[keyFlag][q][0].ToString() != currentStep)
                        {
                            y++;
                            currentStep = AllNotes[keyFlag][q][0].ToString();
                        }
                        if (AllNotes[keyFlag][q] == orderList[i])
                        {
                            y = -10 + (12 * y);
                            Console.WriteLine(orderList[i] + " has " + "y= " + y.ToString());
                            break;
                        }
                    }
                }
                else if (orderIndex > MiddleIndex) //Move up
                {
                    Console.WriteLine("Moving up");
                    for (int q = MiddleIndex + 1; q < AllNotes[keyFlag].Count; q++)
                    {
                        if (AllNotes[keyFlag][q][0].ToString() != currentStep)
                        {
                            y++;
                            currentStep = AllNotes[keyFlag][q][0].ToString();
                        }
                        if (AllNotes[keyFlag][q] == orderList[i])
                        {
                            y = -10 - (12 * y);
                            break;
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
                    Source = ImageSource.FromFile(keyFlag + ".png"),
                    Aspect = Aspect.AspectFit,
                    Scale = 0.16,
                    TranslationX = (19*i-55)+ xThreshold,
                    TranslationY = y,
                    StyleId = keyFlag + i.ToString()
                };
                topGrid.Children.Insert(topGrid.Children.Count - 3, image);
                AddedViews.Add(image.StyleId, topGrid.Children.First(x => x.StyleId == keyFlag + i.ToString()).Id);
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

            string tempKeyFlag = keyFlag;
            if (iClef == "Treble")
            {
                tempKeyFlag = tempKeyFlag == "Flat" ? "Sharp" : "Flat";
            }

            //Get random note and slide pos
            int selectionCount = random.Next(0, KeyNotes.Count-1);
            string selection = KeyNotes[selectionCount];
            int tempSelectionCount = selectionCount;
            int pos = 0;
            for (int i = 0; i < tromboneConfig.MaxPos.Count; i++)
            {
                if (tempSelectionCount + 1 <= tromboneConfig.MaxPos[i])
                {
                    pos = tromboneConfig.MaxPos[i] - tempSelectionCount;
                    Console.WriteLine(selection + ": " + pos.ToString());
                    break;
                }
                else
                {
                    tempSelectionCount -= tromboneConfig.MaxPos[i];
                }
            }

            //Display note on screen
            List<string> clefList = (List<string>)clefConfig.GetType().GetProperty(iClef).GetValue(clefConfig);
            string allNotesSelection = AllNotes[tempKeyFlag].First(x => x == selection);
            int allNotesSelectionCount = AllNotes[tempKeyFlag].IndexOf(allNotesSelection);
            int StartIndex = AllNotes[tempKeyFlag].IndexOf(clefList[2]); //List[2] is the note of the middle line of the specific clef
            Console.WriteLine(allNotesSelectionCount);
            Console.WriteLine(StartIndex);
            string currentNote = clefList[2][0].ToString();
            int steps = 0;
            if (allNotesSelectionCount < StartIndex) //Move down
            {
                for (int q = StartIndex - 1; q >= 0; q--)
                {
                    if (AllNotes[tempKeyFlag][q][0].ToString() != currentNote)
                    {
                        steps++;
                        currentNote = AllNotes[tempKeyFlag][q][0].ToString();
                    }
                    if (AllNotes[tempKeyFlag][q] == selection)
                    {
                        TranslateNote(steps, false);
                        break;
                    }
                }
            }
            else if (allNotesSelectionCount > StartIndex) //Move up
            {
                for (int q = StartIndex + 1; q < AllNotes[tempKeyFlag].Count; q++)
                {
                    if (AllNotes[tempKeyFlag][q][0].ToString() != currentNote)
                    {
                        steps++;
                        currentNote = AllNotes[tempKeyFlag][q][0].ToString();
                    }
                    if (AllNotes[tempKeyFlag][q] == selection)
                    {
                        TranslateNote(steps, true);
                        break;
                    }
                }
            }

            //Control ledger lines
            Console.WriteLine("Step: " + steps.ToString());
            if (steps >= 6)
            {
                if ((steps - 6) % 2 != 0)
                {
                    if(allNotesSelectionCount > StartIndex)
                    {
                        LedgerLine.TranslationY = Note.TranslationY + 12;
                        LedgerLine.TranslationX = Note.TranslationX;
                    }
                    else
                    {
                        LedgerLine.TranslationY = Note.TranslationY - 12;
                        LedgerLine.TranslationX = Note.TranslationX;
                    }
                }
                else
                {
                    LedgerLine.TranslationY = Note.TranslationY;
                    LedgerLine.TranslationX = Note.TranslationX;
                }
                LedgerLine.IsVisible = true;
                if ((steps-6)%2 == 0 && steps>7)
                {
                    for (int i=1; i<((steps-6)/2)+1; i++) 
                    {
                        int temp1 = 0;
                        if (allNotesSelectionCount < StartIndex)
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
                            StyleId = "LedgerLine" +i.ToString()
                        };
                        topGrid.Children.Insert(topGrid.Children.Count-2, image);
                        AddedViews.Add(image.StyleId, topGrid.Children.First(x => x.StyleId == "LedgerLine" + i.ToString()).Id);
                    }
                }
            }
            else
            {
                LedgerLine.IsVisible = false;
            }

            //Set answers
            List<string> answers = new List<string>();
            List<string> wrongAnswers = new List<string>() { "1", "2", "3", "4", "5", "6", "7" };
            answers.Add(pos.ToString());
            wrongAnswers.Remove(pos.ToString());
            for (int i=0; i<3; i++)
            {
                string selected = wrongAnswers[random.Next(0, wrongAnswers.Count-1)];
                answers.Add(selected);
                wrongAnswers.Remove(selected);
            }
            for (int i=0; i<4; i++)
            {
                string selected = answers[random.Next(0, answers.Count - 1)];
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    ButtonStackLayout.FindByName<Button>("SelectionBtn" + (i + 1).ToString()).Text = selected;
                });
                answers.Remove(selected);
            }

            //Set answer to be checked when answered
            currentAnswer = pos.ToString();
            AnswerNote = selection;
            answeredQuestions++;
            await Device.InvokeOnMainThreadAsync(() =>
            {
                ProgressBar.ProgressTo((double)answeredQuestions / settings.questions, 800, Easing.SinOut);
            });

            //Start timer
            timer.Start();
        }
        //Generate notes in KEY in certain range
        private List<string> GetKeyNotes(string keyFlag)
        {
            //Get all notes in the key
            List<string> iOctave = new List<string>((List<string>)tromboneConfig.GetType().GetProperty(keyFlag + "Octave").GetValue(tromboneConfig));
            List<List<string>> ObjAllNotes = new List<List<string>>();
            List<string> CentOct = new List<string>();
            foreach (string item in iOctave)
            {
                bool skip = false;
                if (item.Length > 1)
                {
                    for (int x = 0; x < settings.numOfKey; x++)
                    {
                        List<string> p = (List<string>)clefConfig.GetType().GetProperty("OrderOf" + keyFlag).GetValue(clefConfig);
                        if (item == p[x].Remove(p[x].Length - 1, 1).ToString())
                        {
                            string returned = item + tromboneConfig.CenOct.ToString();
                            CentOct.Add(returned);
                            break;
                        }
                    }
                    continue;
                }
                else
                {
                    for (int x = 0; x < settings.numOfKey; x++)
                    {
                        List<string> p = (List<string>)clefConfig.GetType().GetProperty("OrderOf" + keyFlag).GetValue(clefConfig);
                        if (item == p[x][0].ToString())
                        {
                            skip = true;
                            break;
                        }
                    }
                    if (skip == true)
                    {
                        continue;
                    }
                    string tempItem = item + tromboneConfig.CenOct.ToString();
                    CentOct.Add(tempItem);
                }
            }
            ObjAllNotes.Add(CentOct);
            foreach (List<int> threshold in tromboneConfig.EnlargeRange)
            {
                List<string> tempList = new List<string>(iOctave);
                tempList.RemoveRange(threshold[0], threshold[1]);
                List<string> tempList2 = new List<string>();
                foreach (string item in tempList)
                {
                    bool skip = false;
                    if (item.Length > 1)
                    {
                        for (int x = 0; x < settings.numOfKey; x++)
                        {
                            List<string> p = (List<string>)clefConfig.GetType().GetProperty("OrderOf" + keyFlag).GetValue(clefConfig);
                            if (item == p[x].Remove(p[x].Length - 1, 1).ToString())
                            {
                                string returned = item + threshold[2].ToString();
                                tempList2.Add(returned);
                                break;
                            }
                        }
                        continue;
                    }
                    else
                    {
                        for (int x = 0; x < settings.numOfKey; x++)
                        {
                            List<string> p = (List<string>)clefConfig.GetType().GetProperty("OrderOf" + keyFlag).GetValue(clefConfig);
                            if (item == p[x][0].ToString())
                            {
                                skip = true;
                                break;
                            }
                        }
                        if (skip == true)
                        {
                            continue;
                        }
                        string tempItem = item + threshold[2].ToString();
                        tempList2.Add(tempItem);
                    }
                }
                ObjAllNotes.Add(tempList2);
            }
            List<string> AllNotes = new List<string>();
            for (int i = 0; i < 3; i++)
            {
                int index = Convert.ToInt32((Math.Pow(i - 1, 2)) * ((0.5 * i) + 1)); //Cubic function that returns 1,0,2 from i=0,1,2 respectively
                AllNotes.AddRange(ObjAllNotes[index]);
            }
            Console.WriteLine(String.Join(", ", AllNotes)); //Debug list
            return AllNotes;
        }
        //Move the note in terms of steps, 1 STEP = 1 SEMI-TONE
        private void TranslateNote(int steps, bool up)
        {
            int actualSteps = steps * 12;
            if (up == true)
            {
                Note.TranslationY = -actualSteps;
            }
            else
            {
                Note.TranslationY = actualSteps;
            }
        }
        //Convert JSON to variables in Classes
        private void Deserialisation()
        {
            var assembly = typeof(MainPage).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{"JSON"}.{"TromboneConfig.json"}");
            using (StreamReader file = new StreamReader(stream))
            {
                var json = file.ReadToEnd();
                tromboneConfig = JsonConvert.DeserializeObject<Classes.TromboneConfig>(json);
            }
            stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{"JSON"}.{"ClefConfig.json"}");
            using (StreamReader file = new StreamReader(stream))
            {
                var json = file.ReadToEnd();
                clefConfig = JsonConvert.DeserializeObject<Classes.ClefConfig>(json);
            }
        }
        //Generate chromatic notes in WHOLE range
        private List<string> CreateNotes(string keyFlag)
        {
            //Create a list of all notes (Chromatic scale)
            List<string> iOctave = new List<string>((List<string>)tromboneConfig.GetType().GetProperty(keyFlag + "Octave").GetValue(tromboneConfig));
            List<List<string>> ObjAllNotes = new List<List<string>>();
            List<string> CentOct = new List<string>();
            Console.WriteLine(settings.numOfKey);
            foreach (string item in iOctave)
            {
                string returned = item + tromboneConfig.CenOct.ToString();
                CentOct.Add(returned);
            }
            ObjAllNotes.Add(CentOct);
            foreach (List<int> threshold in tromboneConfig.EnlargeRange)
            {
                List<string> tempList = new List<string>(iOctave);
                tempList.RemoveRange(threshold[0], threshold[1]);
                List<string> tempList2 = new List<string>();
                foreach (string item in tempList)
                {
                    string returned = item + threshold[2].ToString();
                    tempList2.Add(returned);
                }
                ObjAllNotes.Add(tempList2);
            }
            List<string> AllNotes = new List<string>();
            for (int i = 0; i < 3; i++)
            {
                int index = Convert.ToInt32((Math.Pow(i - 1, 2)) * ((0.5 * i) + 1)); //Cubic function that returns 1,0,2 from i=0,1,2 respectively
                AllNotes.AddRange(ObjAllNotes[index]);
            }
            Console.WriteLine(String.Join(", ", AllNotes)); //Debug list
            return AllNotes;
        }
        //SelectionButton Pressed Event
        private void BtnSelectionMade(object sender, EventArgs args)
        {
            Button btn = (Button)sender;
            SelectionMade(btn.Text);
        }
        //Question Timed out Event
        private void HandleTimerElapsed(object sender, ElapsedEventArgs e)
        {
            SelectionMade("TimedOut");
        }
        //Question has been answered event
        private async void SelectionMade(string text)
        {
            if (text == currentAnswer)
            {
                Console.WriteLine("CORRECT: " + AnswerNote + " " + currentAnswer);
                score++;
                if (answeredQuestions == settings.questions)
                {
                    Console.WriteLine(score.ToString() + "/" + settings.questions.ToString());
                    await Application.Current.MainPage.Navigation.PopAsync(false);
                }
                GetNewQuestion();
            }
            else
            {
                Console.WriteLine("INCORRECT: " + AnswerNote + " " + currentAnswer);
                if (answeredQuestions == settings.questions)
                {
                    Console.WriteLine(score.ToString() + "/" + settings.questions.ToString());
                    await Application.Current.MainPage.Navigation.PopAsync(false);
                }
                GetNewQuestion();
            }
        }
    }
}