using System.ComponentModel;
using Android.Content;
using Android.Content.Res;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Picker), typeof(BastelKatalog.Droid.CustomPickerRenderer))]
namespace BastelKatalog.Droid
{
    public class CustomPickerRenderer : PickerRenderer
    {
        public CustomPickerRenderer(Context context)
            : base(context)
        { }

        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            if (Control != null && e.NewElement != null)
            {
                Picker picker = e.NewElement;
                Control.BackgroundTintList = ColorStateList.ValueOf(picker.TextColor.ToAndroid());
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (sender is Picker picker && e.PropertyName == "TextColor")
            {
                Control.BackgroundTintList = ColorStateList.ValueOf(picker.TextColor.ToAndroid());
            }
        }
    }
}