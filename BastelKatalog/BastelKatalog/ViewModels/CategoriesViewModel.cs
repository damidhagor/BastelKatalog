using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xamarin.Forms;

namespace BastelKatalog.ViewModels
{
    public class CategoriesViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged = default!;
        private void NotifyPropertyChanged([CallerMemberName] string caller = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        #endregion


        private readonly Data.CatalogueContext _CatalogueDb;

        public Command<Data.Category> DeleteCommand { get; set; }

        private ObservableCollection<Data.Category> _Categories = new ObservableCollection<Data.Category>();
        public ObservableCollection<Data.Category> Categories
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


        public CategoriesViewModel()
        {
            _CatalogueDb = DependencyService.Resolve<Data.CatalogueContext>();
            DeleteCommand = new Command<Data.Category>(async (c) => await DeleteCategory(c));
        }


        public async Task LoadData()
        {
            try
            {
                List<Data.Category> categories = await _CatalogueDb.Categories.OrderBy(c => c.Name).ToListAsync();
                await Device.InvokeOnMainThreadAsync(() => Categories = new ObservableCollection<Data.Category>(categories));
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error loading categories: {e.Message}");
            }
        }

        public async Task AddCategory(string name)
        {
            try
            {
                _CatalogueDb.Categories.Add(new Data.Category(name));
                await _CatalogueDb.SaveChangesAsync();
                await LoadData();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error creating new category: {e.Message}");
            }
        }

        public async Task EditCategory(Data.Category category, string name)
        {
            try
            {
                category.Name = name;
                await _CatalogueDb.SaveChangesAsync();
                await LoadData();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error editing category: {e.Message}");
            }
        }

        public async Task DeleteCategory(Data.Category category)
        {
            try
            {
                IEnumerable<Data.Item> items = _CatalogueDb.Items.Where(i => i.CategoryId == category.Id);
                foreach (Data.Item item in items)
                {
                    item.CategoryId = null;
                    item.Category = null;
                }

                _CatalogueDb.Categories.Remove(category);
                await _CatalogueDb.SaveChangesAsync();
                await Device.InvokeOnMainThreadAsync(() => Categories.Remove(category));
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error deleting category: {e.Message}");
            }
        }
    }
}
