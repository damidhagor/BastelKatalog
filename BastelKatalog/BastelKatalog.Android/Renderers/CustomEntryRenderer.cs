using System.ComponentModel;
using Android.Content;
using Android.Content.Res;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Entry), typeof(BastelKatalog.Droid.CustomEntryRenderer))]
namespace BastelKatalog.Droid
{
    public class CustomEntryRenderer : EntryRenderer
    {
        public CustomEntryRenderer(Context context)
            : base(context)
        { }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null && e.NewElement != null)
            {
                Entry entry = e.NewElement;
                Control.BackgroundTintList = ColorStateList.ValueOf(entry.TextColor.ToAndroid());
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (sender is Entry entry && e.PropertyName == "TextColor")
            {
                Control.BackgroundTintList = ColorStateList.ValueOf(entry.TextColor.ToAndroid());
            }
        }
    }
}