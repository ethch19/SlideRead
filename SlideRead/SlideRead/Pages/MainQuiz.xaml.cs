using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Timers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        List<string> AllNotes = new List<string>();
        Timer timer;
        string currentAnswer = String.Empty;
        string AnswerNote = String.Empty;
        int score = 0;
        int answeredQuestions = 0;
        public MainQuiz()
        {
            InitializeComponent();
            Deserialisation();
            AllNotes = CreateNotes(tromboneConfig);
        }
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
            Console.WriteLine(iClef);
            if (iClef == "Mixed")
            {
                iClef = new string[] { "Treble", "Bass", "Tenor" }[random.Next(0, 2)];
            }
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

            //Get random note and slide pos
            int selectionCount = random.Next(0, AllNotes.Count-1);
            string selection = AllNotes[selectionCount];
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
            int StartIndex = AllNotes.IndexOf(clefList[2]); //List[2] is the note of the middle line of the specific clef
            Console.WriteLine(StartIndex);
            string currentNote = clefList[2][0].ToString();
            int steps = 0;
            if (selectionCount < StartIndex) //Move down
            {
                for (int q = AllNotes.IndexOf(clefList[2]) - 1; q >= 0; q--)
                {
                    if (AllNotes[q][0].ToString() != currentNote)
                    {
                        steps++;
                        currentNote = AllNotes[q][0].ToString();
                    }
                    if (AllNotes[q] == selection)
                    {
                        TranslateNote(steps, false);
                        break;
                    }
                }
            }
            else if (selectionCount > StartIndex) //Move up
            {
                for (int q = AllNotes.IndexOf(clefList[2]) + 1; q < AllNotes.Count; q++)
                {
                    if (AllNotes[q][0].ToString() != currentNote)
                    {
                        steps++;
                        currentNote = AllNotes[q][0].ToString();
                    }
                    if (AllNotes[q] == selection)
                    {
                        TranslateNote(steps, true);
                        break;
                    }
                }
            }

            //Control ledger lines
            Console.WriteLine(selectionCount);
            for (int i = 0; i<AddedViews.Count; i++)
            {
                topGrid.Children.Remove(topGrid.Children.First(x => x.StyleId == "LedgerLine"+ (i+1).ToString()));
                AddedViews.Remove("LedgerLine" + (i + 1).ToString());
            }
            Console.WriteLine("Step: " + steps.ToString());
            if (steps >= 6)
            {
                if ((steps - 6) % 2 != 0)
                {
                    if(selectionCount>StartIndex)
                    {
                        LedgerLine.TranslationY = Note.TranslationY + 12;
                    }
                    else
                    {
                        LedgerLine.TranslationY = Note.TranslationY - 12;
                    }
                }
                else
                {
                    LedgerLine.TranslationY = Note.TranslationY;
                }
                LedgerLine.IsVisible = true;
                if ((steps-6)%2 == 0 && steps>7)
                {
                    for (int i=1; i<((steps-6)/2)+1; i++) 
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
                            TranslationX = 50,
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
        //Convert JSON to Class
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
        //Generate range of notes into a list
        private List<string> CreateNotes(Classes.TromboneConfig tromboneConfig)
        {
            List<List<string>> ObjAllNotes = new List<List<string>>();
            List<string> CentOct = new List<string>();
            foreach (string item in tromboneConfig.Octave)
            {
                string tempItem = item + tromboneConfig.CenOct.ToString();
                CentOct.Add(tempItem);
            }
            ObjAllNotes.Add(CentOct);
            foreach (List<int> threshold in tromboneConfig.EnlargeRange)
            {
                List<string> tempList = new List<string>(tromboneConfig.Octave);
                tempList.RemoveRange(threshold[0], threshold[1]);
                List<string> tempList2 = new List<string>();
                foreach (string item in tempList)
                {
                    string tempItem = item + threshold[2].ToString();
                    tempList2.Add(tempItem);
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
        //Button Pressed Event
        private void BtnSelectionMade(object sender, EventArgs args)
        {
            Button btn = (Button)sender;
            SelectionMade(btn.Text);
        }
        //Timed out Event
        private void HandleTimerElapsed(object sender, ElapsedEventArgs e)
        {
/*            SelectionMade("TimedOut");*/
        }
        //Question has been answered
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