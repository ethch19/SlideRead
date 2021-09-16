using Android.App;
using Android.Content;
using Android.OS;
using Android.Transitions;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.View;
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
            if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
            {
                Window.Attributes.LayoutInDisplayCutoutMode = LayoutInDisplayCutoutMode.ShortEdges;
            }
            Window.AddFlags(WindowManagerFlags.Fullscreen);
            Window.AddFlags(WindowManagerFlags.TranslucentNavigation);
            
            SetContentView(Resource.Layout.SplashLayout);
            if((int)Build.VERSION.SdkInt >= 30)
            {
                Window.SetDecorFitsSystemWindows(true);
                IWindowInsetsController insetsController = Window.InsetsController;
                if (insetsController != null)
                {
                    insetsController.Hide(WindowInsets.Type.NavigationBars());
                }
            }
            else
            {
                var uiOptions = (int)Window.DecorView.SystemUiVisibility;
                var newUiOptions = (int)uiOptions;

                newUiOptions |=
                    (int)SystemUiFlags.LayoutStable |
                    (int)SystemUiFlags.LayoutHideNavigation |
                    (int)SystemUiFlags.LayoutFullscreen |
                    (int)SystemUiFlags.HideNavigation |
                    (int)SystemUiFlags.Fullscreen |
                    (int)SystemUiFlags.ImmersiveSticky;

                Window.DecorView.SystemUiVisibility = (StatusBarVisibility)newUiOptions;
            }
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