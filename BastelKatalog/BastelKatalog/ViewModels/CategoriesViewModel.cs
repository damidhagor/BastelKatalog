using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BastelKatalog.Models;
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

        public Command<CategoryWrapper> DeleteCommand { get; set; }

        private ObservableCollection<CategoryWrapper> _Categories = new ObservableCollection<CategoryWrapper>();
        public ObservableCollection<CategoryWrapper> Categories
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
            DeleteCommand = new Command<CategoryWrapper>(async (c) => await DeleteCategory(c));
        }


        public async Task LoadData()
        {
            try
            {
                List<Data.Category> categories = await _CatalogueDb.Categories.OrderBy(c => c.Name).ToListAsync();
                ObservableCollection<CategoryWrapper> categoryWrappers = BuildCategoriesList(categories);
                await Device.InvokeOnMainThreadAsync(() => Categories = categoryWrappers);
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

        public async Task AddSubCategory(CategoryWrapper category, string name)
        {
            try
            {
                _CatalogueDb.Categories.Add(new Data.Category(name)
                {
                    ParentCategory = category.Category,
                    ParentCategoryId = category.Category.Id
                });
                await _CatalogueDb.SaveChangesAsync();
                await LoadData();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error creating new sub category: {e.Message}");
            }
        }

        public async Task EditCategory(CategoryWrapper category, string name)
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

        public async Task DeleteCategory(CategoryWrapper category)
        {
            try
            {
                IEnumerable<Data.Item> items = _CatalogueDb.Items.Where(i => i.CategoryId == category.Category.Id);
                foreach (Data.Item item in items)
                {
                    item.CategoryId = null;
                    item.Category = null;
                }

                _CatalogueDb.Categories.Remove(category.Category);
                await _CatalogueDb.SaveChangesAsync();
                await LoadData();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error deleting category: {e.Message}");
            }
        }


        /// <summary>
        /// Builds the category list and calculates SubCategoryPositions
        /// </summary>
        private ObservableCollection<CategoryWrapper> BuildCategoriesList(List<Data.Category> categories)
        {
            // Group categories by their parent Ids
            Dictionary<int, List<Data.Category>> categoriesByParentId = categories
                .GroupBy(c => c.ParentCategoryId ?? -1)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Builds category list recursively for a parent category
            List<CategoryWrapper> BuildSubList(CategoryWrapper? parentCategory)
            {
                // Get sub categories
                List<CategoryWrapper> wrappers = new List<CategoryWrapper>();
                if (!categoriesByParentId.TryGetValue(parentCategory?.Category?.Id ?? -1, out List<Data.Category> subCategories)
                    || (subCategories?.Count ?? 0) < 1)
                    return wrappers;

                // Calculate positions ordered by name
                List<Data.Category> orderedSubCategories = subCategories.OrderBy(c => c.Name).ToList();
                for (int i = 0; i < orderedSubCategories.Count; i++)
                {
                    CategoryWrapper wrapper = new CategoryWrapper(orderedSubCategories[i]);
                    wrapper.ParentCategory = parentCategory;
                    wrapper.SubCategoryPosition = (parentCategory == null) ? SubCategoryPosition.None :
                        (i == 0 && orderedSubCategories.Count > 1) ? SubCategoryPosition.Start
                        : (i < orderedSubCategories.Count - 1) ? SubCategoryPosition.Middle
                        : SubCategoryPosition.End;

                    wrappers.Add(wrapper);
                    // Calculate positions of its sub categories (recursive)
                    wrappers.AddRange(BuildSubList(wrapper));
                }

                return wrappers;
            }

            // Build/Start for main categories (no parents)
            return new ObservableCollection<CategoryWrapper>(BuildSubList(null));
        }
    }
}
