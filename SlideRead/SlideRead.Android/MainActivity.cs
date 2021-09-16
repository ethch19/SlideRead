using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;

namespace SlideRead.Droid
{
    [Activity(Label = "SlideRead", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize, ScreenOrientation =ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
            if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
            {
                Window.Attributes.LayoutInDisplayCutoutMode = LayoutInDisplayCutoutMode.ShortEdges;
            }
            Window.AddFlags(WindowManagerFlags.Fullscreen);
            Window.AddFlags(WindowManagerFlags.TranslucentNavigation);
            if ((int)Build.VERSION.SdkInt >= 30)
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
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}