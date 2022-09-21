using Android.Content;
using Android.Graphics.Drawables;
using SlideRead.Controls;
using SlideRead.Droid.CustomRenderers;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomLabel), typeof(CustomLabelRenderer))]
namespace SlideRead.Droid.CustomRenderers
{
    public class CustomLabelRenderer : LabelRenderer
    {
        public CustomLabelRenderer(Context context) : base(context)
        {

        }
        CustomLabel customLabel;
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                customLabel = (CustomLabel)e.NewElement;
                var gradientDrawable = new GradientDrawable();
                gradientDrawable.SetCornerRadius(customLabel.LabelCornerRadius);
                gradientDrawable.SetColor(customLabel.LabelBackgroundColor.ToAndroid());
                gradientDrawable.SetAlpha((int)Math.Round(255 * customLabel.LabelBackgroundTransparency));
                gradientDrawable.SetStroke(customLabel.LabelBorderWidth, customLabel.LabelBorderColor.ToAndroid());
                Control.SetBackground(gradientDrawable);
                Control.SetAllCaps(false);
            }
        }
    }
}