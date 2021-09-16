using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SlideRead.Controls
{
    public class CustomStackLayout : StackLayout
    {
        public static readonly BindableProperty CustomCornerRadiusProperty = BindableProperty.Create("CornerRadius", typeof(CornerRadius), typeof(CustomStackLayout), new CornerRadius(0,0,0,0));
        
        public CornerRadius SLCornerRadius
        {
            get { return (CornerRadius)GetValue(CustomCornerRadiusProperty); }
            set { SetValue(CustomCornerRadiusProperty, value); }
        }

        public static readonly BindableProperty CustomBorderWidthProperty = BindableProperty.Create("BorderWidthProperty", typeof(int), typeof(CustomStackLayout), 0);

        public int SLBorderWidth
        {
            get { return (int)GetValue(CustomBorderWidthProperty); }
            set { SetValue(CustomBorderWidthProperty, value); }
        }

        public static readonly BindableProperty CustomBorderColorProperty = BindableProperty.Create("BorderColorProperty", typeof(Color), typeof(CustomStackLayout), Color.Transparent);

        public Color SLBorderColor
        {
            get { return (Color)GetValue(CustomBorderColorProperty); }
            set { SetValue(CustomBorderColorProperty, value); }
        }

        public static readonly BindableProperty CustomBackgroundColorProperty = BindableProperty.Create("BackgroundColorProperty", typeof(Color), typeof(CustomStackLayout), Color.Transparent);

        public Color SLBackgroundColor
        {
            get { return (Color)GetValue(CustomBackgroundColorProperty); }
            set { SetValue(CustomBackgroundColorProperty, value); }
        }
    }
}
