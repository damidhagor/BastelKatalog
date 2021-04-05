using BastelKatalog.Data;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace BastelKatalog.Models
{
    public class ItemImage : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged = default!;
        private void NotifyPropertyChanged([CallerMemberName] string caller = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        #endregion

        private string _ImagePath;
        public string ImagePath
        {
            get { return _ImagePath; }
            set
            {
                if (value != _ImagePath)
                {
                    _ImagePath = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ImageSource _ImageSource;
        public ImageSource ImageSource
        {
            get { return _ImageSource; }
            set
            {
                if (value != _ImageSource)
                {
                    _ImageSource = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _IsNew;
        public bool IsNew
        {
            get { return _IsNew; }
            set
            {
                if (value != _IsNew)
                {
                    _IsNew = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private byte[]? _ImageData;
        public byte[]? ImageData
        {
            get { return _ImageData; }
            set
            {
                if (value != _ImageData)
                {
                    _ImageData = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsDefault => ImagePath == String.Empty && !IsNew;


        public ItemImage(string path)
        {
            _ImagePath = path;
            _ImageSource = ImageManager.GetImage(ImagePath);
            _IsNew = false;
            _ImageData = null;
        }

        public ItemImage(byte[] imageData)
        {
            _ImagePath = String.Empty;
            _IsNew = true;
            _ImageData = imageData;
            _ImageSource = ImageSource.FromStream(() => new MemoryStream(ImageData, false));
        }
    }
}
