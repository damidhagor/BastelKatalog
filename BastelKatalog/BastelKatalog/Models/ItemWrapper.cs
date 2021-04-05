using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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

        public static readonly string ImagePathSeperator = ":";

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

        public Category? Category
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

        private ObservableCollection<ItemImage> _Images;
        public ObservableCollection<ItemImage> Images
        {
            get { return _Images; }
            set
            {
                if (value != _Images)
                {
                    _Images = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(ImageCount));
                    NotifyPropertyChanged(nameof(SelectedImageIndex));
                }
            }
        }

        private ItemImage? _SelectedImage;
        public ItemImage? SelectedImage
        {
            get { return _SelectedImage; }
            set
            {
                if (value != _SelectedImage)
                {
                    _SelectedImage = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(SelectedImageIndex));
                }
            }
        }

        public int ImageCount => Images.Count;
        public int SelectedImageIndex => Images.IndexOf(SelectedImage) + 1;


        public ItemWrapper(Item item)
        {
            Item = item;
            _Images = new ObservableCollection<ItemImage>();
            LoadImages();
        }


        public void AddNewImage(byte[] imageData)
        {
            ItemImage image = new ItemImage(imageData);
            Images.Add(image);
            SelectedImage = image;

            NotifyPropertyChanged(nameof(ImageCount));
        }

        public void DeleteImage(ItemImage image)
        {
            Images.Remove(image);

            SelectedImage = Images.Count == 0 ? null : Images[Math.Min(SelectedImageIndex, ImageCount - 1)];
            NotifyPropertyChanged(nameof(ImageCount));
        }

        public async Task SaveImages()
        {
            foreach (ItemImage image in Images)
            {
                if (image.IsNew && image.ImageData != null)
                    image.ImagePath = await ImageManager.SaveImage(image.ImageData) ?? String.Empty;
            }

            if (!String.IsNullOrWhiteSpace(Item.ImagePath))
            {
                string[] oldImagePaths = Item.ImagePath.Split(ImagePathSeperator);
                foreach (string imagePath in oldImagePaths)
                    if (!Images.Any(i => i.ImagePath == imagePath))
                        ImageManager.DeleteImage(imagePath);
            }

            Item.ImagePath = String.Join(ImagePathSeperator, Images.Select(i => i.ImagePath).Where(p => !String.IsNullOrWhiteSpace(p)).ToList());
        }


        private void LoadImages()
        {
            Images.Clear();

            if (!String.IsNullOrWhiteSpace(Item.ImagePath))
            {
                string[] imagePaths = Item.ImagePath.Split(ImagePathSeperator);
                foreach (string imagePath in imagePaths)
                    Images.Add(new ItemImage(imagePath));

                if (Images.Count > 0)
                    SelectedImage = Images[0];
            }
        }
    }
}