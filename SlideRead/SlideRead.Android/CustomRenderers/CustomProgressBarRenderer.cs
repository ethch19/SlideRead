using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using SlideRead.Controls;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;
using SlideRead.Droid.CustomRenderers;

[assembly: ExportRenderer(typeof(CustomProgressBar), typeof(CustomProgressBarRenderer))]
namespace SlideRead.Droid.CustomRenderers
{
    public class CustomProgressBarRenderer : ProgressBarRenderer
    {
        public CustomProgressBarRenderer(Context context) : base(context)
        {

        }
        CustomProgressBar progressBar;
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ProgressBar> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                progressBar = (CustomProgressBar)e.NewElement;
                if ((int)Build.VERSION.SdkInt >= 29)
                {
                    Control.ProgressTintBlendMode = Android.Graphics.BlendMode.SrcIn;
                    Control.ProgressBackgroundTintBlendMode = Android.Graphics.BlendMode.SrcOut;
                }
                else
                {
                    Control.ProgressBackgroundTintMode = Android.Graphics.PorterDuff.Mode.SrcIn;
                    Control.BackgroundTintMode = Android.Graphics.PorterDuff.Mode.SrcOut;
                }
                Control.ProgressBackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Color.FromHex("#000000").ToAndroid());
                Control.ProgressTintList = Android.Content.Res.ColorStateList.ValueOf(Color.FromHex("#4B54AA").ToAndroid());
                Control.ScaleY = 3.8F;
            }
        }
    }
}