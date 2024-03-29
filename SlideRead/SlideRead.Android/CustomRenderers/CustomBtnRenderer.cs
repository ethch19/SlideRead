﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlideRead.Controls;
using SlideRead.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;
using System.ComponentModel;
using Android.Text;

[assembly: ExportRenderer(typeof(CustomBtn), typeof(CustomBtnRenderer))]
namespace SlideRead.Droid.CustomRenderers
{
    public class CustomBtnRenderer : ButtonRenderer
    {
        public CustomBtnRenderer(Context context) : base(context)
        {

        }
        CustomBtn customBtn;
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                customBtn = (CustomBtn)e.NewElement;
                var gradientDrawable = new GradientDrawable();
                gradientDrawable.SetCornerRadius(customBtn.ButtonCornerRadius);
                gradientDrawable.SetColor(customBtn.ButtonBackgroundColor.ToAndroid());
                gradientDrawable.SetStroke(customBtn.ButtonBorderWidth, customBtn.ButtonBorderColor.ToAndroid());
                Control.SetBackground(gradientDrawable);
                Control.SetAllCaps(false);
                if (customBtn.ButtonHtml == true)
                {
                    Control.SetText(Html.FromHtml(customBtn.Text), Android.Widget.TextView.BufferType.Spannable);
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Control != null)
            {
                var gradientDrawable = new GradientDrawable();
                gradientDrawable.SetCornerRadius(customBtn.ButtonCornerRadius);
                gradientDrawable.SetColor(customBtn.ButtonBackgroundColor.ToAndroid());
                gradientDrawable.SetStroke(customBtn.ButtonBorderWidth, customBtn.ButtonBorderColor.ToAndroid());
                Control.SetBackground(gradientDrawable);
                Control.SetAllCaps(false);
                if (customBtn.ButtonHtml == true)
                {
                    Control.SetText(Html.FromHtml(customBtn.Text), Android.Widget.TextView.BufferType.Spannable);
                }
                this.Invalidate();
            }
        }
    }
}