using System;
using Xamarin.Forms;
namespace SlideRead
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        private async void SettingsBtnClicked(object sender, EventArgs args)
        {
            SettingsButton.IsEnabled = false;
            await Application.Current.MainPage.Navigation.PushAsync(new Pages.SettingsPage(), false);
            SettingsButton.IsEnabled = true;
        }
        private async void TromboneBtnClicked(object sender, EventArgs args)
        {
            TromboneButton.IsEnabled = false;
            await Application.Current.MainPage.Navigation.PushAsync(new Pages.MainQuiz(), false);
            TromboneButton.IsEnabled = true;
        }
        private async void GeneralBtnClicked(object sender, EventArgs args)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new Pages.GeneralQuiz(), false);
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            grid.Opacity = 0;
            grid.FadeTo(1, 350);
        }
    }
}
