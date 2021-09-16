using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SlideRead.Controls
{
    public class CustomProgressBar : ProgressBar
    {
        public static readonly BindableProperty CustomProgressBarCornerRadius = BindableProperty.Create("CornerRadius", typeof(float), typeof(CustomBtn), 0F);
        public float ProgressBarCornerRadius
        {
            get { return (float)GetValue(CustomProgressBarCornerRadius); }
            set { SetValue(CustomProgressBarCornerRadius, value); }
        }
    }
}