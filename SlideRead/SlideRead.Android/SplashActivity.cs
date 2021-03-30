using Android.App;
using Android.Content;
using Android.OS;
using Android.Transitions;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.Airbnb.Lottie;
using Android.Animation;

namespace SlideRead.Droid
{
    [Activity(Theme= "@style/MainTheme.SplashScreen", MainLauncher = true, NoHistory = true, Label = "SplashActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SplashActivity : Activity, Animator.IAnimatorListener
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SplashLayout);
            LottieAnimationView animationView = FindViewById<LottieAnimationView>(Resource.Id.animation_view);
            Window.ExitTransition = new Fade();
            animationView.AddAnimatorListener(this);
            
        }
        public void OnAnimationEnd(Animator animation)
        {
            StartActivity(new Intent(this, typeof(MainActivity)), ActivityOptions.MakeSceneTransitionAnimation(this).ToBundle());
        }
        public void OnAnimationCancel(Animator animation) { }
        public void OnAnimationRepeat(Animator animation) { }
        public void OnAnimationStart(Animator animation) { }
        public override void OnBackPressed() { }
    }
}