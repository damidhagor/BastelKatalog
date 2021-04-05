using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BastelKatalog.Data;
using BastelKatalog.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Xamarin.Forms;

namespace BastelKatalog.ViewModels
{
    public class EditItemViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged = default!;
        private void NotifyPropertyChanged([CallerMemberName] string caller = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        #endregion

        private readonly CatalogueContext _CatalogueDb;

        public bool WasSaved { get; private set; }

        private ItemWrapper _Item;
        public ItemWrapper Item
        {
            get { return _Item; }
            set
            {
                if (value != _Item)
                {
                    _Item = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public byte[]? ImageData;

        private ObservableCollection<Category> _Categories = new ObservableCollection<Data.Category>();
        public ObservableCollection<Category> Categories
        {
            get { return _Categories; }
            set
            {
                if (value != _Categories)
                {
                    _Categories = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public EditItemViewModel()
        {
            _CatalogueDb = DependencyService.Resolve<CatalogueContext>();
            _Item = new ItemWrapper(new Item(""));
        }


        public void LoadData(int itemId)
        {
            try
            {
                Categories = new ObservableCollection<Category>(_CatalogueDb.Categories.ToList());

                Item = _CatalogueDb.Items.FirstOrDefault(i => i.Id == itemId)?.ToItemWrapper()
                    ?? new ItemWrapper(new Item(""));
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error loading item: {e.Message}");
            }
        }

        public void Reset()
        {
            Item = new ItemWrapper(new Item(""));
        }

        public string? Validate()
        {
            // Name must be entered
            if (String.IsNullOrWhiteSpace(Item.Name))
                return "Bitte Namen angeben.";
            // Stock must not be negative
            if (Item.Stock < 0f)
                return "Menge darf nicht negativ sein.";

            return null;
        }

        public void SetImageData(byte[] data)
        {
            try
            {
                using (SkiaSharp.SKBitmap original = SkiaSharp.SKBitmap.Decode(data))
                {
                    // Calculate new image size so biggest side is 1000 pixels
                    int width, height;
                    if (original.Width == original.Height)
                    {
                        width = height = 1000;
                    }
                    else if (original.Width > original.Height)
                    {
                        width = 1000;
                        height = (int)(1000 * ((float)original.Height / original.Width));
                    }
                    else
                    {
                        height = 1000;
                        width = (int)(1000 * ((float)original.Width / original.Height));
                    }

                    // Resize image and write to ImageData
                    using (SkiaSharp.SKBitmap resized = new SkiaSharp.SKBitmap(width, height, original.ColorType, original.AlphaType, original.ColorSpace))
                    {
                        if (original.ScalePixels(resized, SkiaSharp.SKFilterQuality.High))
                        {
                            using (MemoryStream stream = new MemoryStream())
                            {
                                if (resized.Encode(stream, SkiaSharp.SKEncodedImageFormat.Jpeg, 100))
                                {
                                    ImageData = stream.ToArray();
                                    Item.Image = ImageSource.FromStream(() => new MemoryStream(ImageData, false));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error loading image: {e.Message}");
            }
        }

        public async Task<bool> SaveItem()
        {
            try
            {
                // Save image and delete existing if overwritten
                if (ImageData != null)
                {
                    string? filename = await ImageManager.SaveImage(ImageData);
                    if (!String.IsNullOrWhiteSpace(filename))
                    {
                        if (Item.Item.ImagePath != null)
                            ImageManager.DeleteImage(Item.Item.ImagePath);

                        Item.Item.ImagePath = filename;
                    }
                }

                if (_Item.Item.Id == 0)
                    _CatalogueDb.Items.Add(_Item.Item);
                await _CatalogueDb.SaveChangesAsync();

                WasSaved = true;

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error saving item: {e.Message}");
                return false;
            }
        }

        public void RevertChanges()
        {
            EntityEntry<Item>? entry = _CatalogueDb.ChangeTracker.Entries<Item>().FirstOrDefault(i => i.Entity.Id == Item.Item.Id);
            if (entry == null || entry.State == EntityState.Unchanged)
                return;

            entry.CurrentValues.SetValues(entry.OriginalValues);
            entry.State = EntityState.Unchanged;
        }
    }
}
