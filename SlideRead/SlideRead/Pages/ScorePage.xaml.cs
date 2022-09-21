using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SlideRead.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScorePage : ContentPage
    {
        public ScorePage(int score, int totalScore)
        {
            InitializeComponent();
            ScoreLabel.Text = score.ToString();
            TotalLabel.Text = "Out of " + totalScore.ToString();
        }
        private async void BackBtnClicked(object sender, EventArgs args)
        {
            MenuButton.IsEnabled = false;
            StartButton.IsEnabled = false;
            await Navigation.PopToRootAsync();
            StartButton.IsEnabled = true;
            MenuButton.IsEnabled = true;
        }

        private async void StartBtnClicked(object sender, EventArgs args)
        {
            StartButton.IsEnabled = false;
            MenuButton.IsEnabled = false;
            var _navigation = Application.Current.MainPage.Navigation;
            var lastSecondPage = _navigation.NavigationStack[_navigation.NavigationStack.Count - 2];
            _navigation.RemovePage(lastSecondPage);
            await _navigation.PushAsync(new MainQuiz());
            lastSecondPage = _navigation.NavigationStack[_navigation.NavigationStack.Count - 2];
            _navigation.RemovePage(lastSecondPage);
            StartButton.IsEnabled = true;
            MenuButton.IsEnabled = true;
        }
    }
}