using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BastelKatalog.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewImagePopupPage : PopupPage
    {
        #region INotifyPropertyChanged
        private void NotifyPropertyChanged([CallerMemberName] string caller = "")
            => OnPropertyChanged(caller);
        #endregion

        private string _ImageTitle;
        public string ImageTitle
        {
            get { return _ImageTitle; }
            set
            {
                if (value != _ImageTitle)
                {
                    _ImageTitle = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ImageSource _Image;
        public ImageSource Image
        {
            get { return _Image; }
            set
            {
                if (value != _Image)
                {
                    _Image = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public ViewImagePopupPage(string title, ImageSource image)
        {
            InitializeComponent();

            _ImageTitle = title;
            _Image = image;

            BindingContext = this;
        }


        public async void Close_Tapped(object sender, EventArgs e)
        {
            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
        }
    }
}