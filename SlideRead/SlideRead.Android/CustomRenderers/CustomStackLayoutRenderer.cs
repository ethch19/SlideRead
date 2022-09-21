using Android.Content;
using Android.Graphics.Drawables;
using SlideRead.Controls;
using SlideRead.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomStackLayout), typeof(CustomStackLayoutRenderer))]
namespace SlideRead.Droid.CustomRenderers
{
    public class CustomStackLayoutRenderer : VisualElementRenderer<StackLayout>
    {
        public CustomStackLayoutRenderer(Context context) : base(context)
        {

        }
        protected override void OnElementChanged(ElementChangedEventArgs<StackLayout> e)
        {
            base.OnElementChanged(e);

            if (this != null)
            {
                CustomStackLayout customSL = (CustomStackLayout)e.NewElement;
                var gradientDrawable = new GradientDrawable();
                float[] CornerRadiusArray = new float[8];
                CornerRadiusArray[0] = (float)customSL.SLCornerRadius.TopLeft;
                CornerRadiusArray[1] = (float)customSL.SLCornerRadius.TopLeft;
                CornerRadiusArray[2] = (float)customSL.SLCornerRadius.TopRight;
                CornerRadiusArray[3] = (float)customSL.SLCornerRadius.TopRight;
                CornerRadiusArray[4] = (float)customSL.SLCornerRadius.BottomRight;
                CornerRadiusArray[5] = (float)customSL.SLCornerRadius.BottomRight;
                CornerRadiusArray[6] = (float)customSL.SLCornerRadius.BottomLeft;
                CornerRadiusArray[7] = (float)customSL.SLCornerRadius.BottomLeft;
                gradientDrawable.SetCornerRadii(CornerRadiusArray);
                gradientDrawable.SetStroke(customSL.SLBorderWidth, customSL.SLBorderColor.ToAndroid());
                gradientDrawable.SetColor(customSL.SLBackgroundColor.ToAndroid());
                this.SetBackground(gradientDrawable);
            }
        }
    }
}