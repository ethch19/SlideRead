using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SlideRead.Controls
{
    public class CustomLabel : Label
    {
        public static readonly BindableProperty CustomCornerRadiusProperty = BindableProperty.Create("CornerRadius", typeof(float), typeof(CustomBtn), 0F);

        public float LabelCornerRadius
        {
            get { return (float)GetValue(CustomCornerRadiusProperty); }
            set { SetValue(CustomCornerRadiusProperty, value); }
        }

        public static readonly BindableProperty CustomBackgroundColorProperty = BindableProperty.Create("BackgroundColorProperty", typeof(Color), typeof(CustomBtn), Color.Transparent);

        public Color LabelBackgroundColor
        {
            get { return (Color)GetValue(CustomBackgroundColorProperty); }
            set { SetValue(CustomBackgroundColorProperty, value); }
        }

        public static readonly BindableProperty CustomBorderWidthProperty = BindableProperty.Create("BorderWidthProperty", typeof(int), typeof(CustomBtn), 0);

        public int LabelBorderWidth
        {
            get { return (int)GetValue(CustomBorderWidthProperty); }
            set { SetValue(CustomBorderWidthProperty, value); }
        }

        public static readonly BindableProperty CustomBorderColorProperty = BindableProperty.Create("BorderColorProperty", typeof(Color), typeof(CustomBtn), Color.Transparent);

        public Color LabelBorderColor
        {
            get { return (Color)GetValue(CustomBorderColorProperty); }
            set { SetValue(CustomBorderColorProperty, value); }
        }
    }
}
