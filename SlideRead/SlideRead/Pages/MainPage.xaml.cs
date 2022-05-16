using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
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
            await Application.Current.MainPage.Navigation.PushAsync(new Pages.SettingsPage(), false);
        }
        private async void TromboneBtnClicked(object sender, EventArgs args)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new Pages.MainQuiz(), false);
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
