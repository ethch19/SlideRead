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
        List<string> AllNotes = new List<string>();
        Timer timer;
        string currentAnswer = String.Empty;
        string AnswerNote = String.Empty;
        int score = 0;
        int answeredQuestions = 0;
        public MainQuiz()
        {
            InitializeComponent();
            tromboneConfig = Deserialisation();
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
            //Get random note and slide pos
            Random random = new Random();
            int selectionCount = random.Next(0, AllNotes.Count-1);
            string selection = AllNotes[selectionCount];
            int tempSelectionCount = selectionCount;
            int pos = 0;
            for (int i = 0; i < tromboneConfig.MaxPos.Count; i++)
            {
                if (tempSelectionCount + 1 <= tromboneConfig.MaxPos[i])
                {
/*                        for (int q = 0; q < i; q++) ONLY USE IF YOU WANT TO CHOOSE TO NOT DUPLICATE QUESTIONS
                    {
                        tempCount = selectionCount - tromboneConfig.MaxPos[q];
                    }*/
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
            int StartIndex = AllNotes.IndexOf("D3");
            Console.WriteLine(StartIndex);
            Console.WriteLine(selectionCount);
            if (selectionCount < StartIndex) //Move down
            {
                string currentNote = "D";
                int steps = 0;
                for (int q = AllNotes.IndexOf("D3") - 1; q >= 0; q--)
                {
                    if (AllNotes[q][0].ToString() != currentNote)
                    {
                        steps++;
                        currentNote = AllNotes[q][0].ToString();
                    }
                    if (AllNotes[q] == selection)
                    {
                        Console.WriteLine(steps);
                        TranslateNote(steps, false);
                        break;
                    }
                }
            }
            else if (selectionCount > StartIndex) //Move up
            {
                string currentNote = "D";
                int steps = 0;
                for (int q = AllNotes.IndexOf("D3") + 1; q < AllNotes.Count; q++)
                {
                    if (AllNotes[q][0].ToString() != currentNote)
                    {
                        steps++;
                        currentNote = AllNotes[q][0].ToString();
                    }
                    if (AllNotes[q] == selection)
                    {
                        Console.WriteLine(steps);
                        TranslateNote(steps, true);
                        break;
                    }
                }
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
                Console.WriteLine("Selected: " + selected);
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    Console.WriteLine("Roger" + i.ToString());
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
        private Classes.TromboneConfig Deserialisation()
        {
            Classes.TromboneConfig tromboneConfig;
            var assembly = typeof(MainPage).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{"TromboneSlidePos.json"}");
            using (StreamReader file = new StreamReader(stream))
            {
                var json = file.ReadToEnd();
                tromboneConfig = JsonConvert.DeserializeObject<Classes.TromboneConfig>(json);
            }
            return tromboneConfig;
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
            SelectionMade("TimedOut");
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
                Console.WriteLine("CORRECT: " + AnswerNote + " " + currentAnswer);
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