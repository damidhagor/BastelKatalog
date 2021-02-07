using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BastelKatalog.Data;
using Xamarin.Forms;

namespace BastelKatalog.Models
{
    /// <summary>
    /// Wraps Item object for UI usage
    /// </summary>
    public class ItemWrapper : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged = default!;
        private void NotifyPropertyChanged([CallerMemberName] string caller = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        #endregion

        public Item Item { get; private set; }

        public string Name
        {
            get { return Item.Name; }
            set
            {
                if (Item.Name != value)
                {
                    Item.Name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Code
        {
            get { return Item.Code ?? ""; }
            set
            {
                if (Item.Code != value)
                {
                    Item.Code = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Data.Category? Category
        {
            get { return Item.Category; }
            set
            {
                if (Item.Category?.Id != value?.Id)
                {
                    Item.CategoryId = value?.Id;
                    Item.Category = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public float Stock
        {
            get { return Item.Stock; }
            set
            {
                if (Item.Stock != value)
                {
                    Item.Stock = Math.Max(0, value);
                    NotifyPropertyChanged();
                }
            }
        }

        public string Description
        {
            get { return Item.Description ?? ""; }
            set
            {
                if (Item.Description != value)
                {
                    Item.Description = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Tags
        {
            get { return Item.Tags ?? ""; }
            set
            {
                if (Item.Tags != value)
                {
                    Item.Tags = value;
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


        public ItemWrapper(Item item)
        {
            Item = item;
            _Image = ImageManager.GetImage(Item.ImagePath);
        }
    }
}