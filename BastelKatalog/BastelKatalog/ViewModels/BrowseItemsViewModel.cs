using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BastelKatalog.Data;
using BastelKatalog.Models;
using BastelKatalog.Views;
using Microsoft.EntityFrameworkCore;
using Xamarin.Forms;

namespace BastelKatalog.ViewModels
{
    public class BrowseItemsViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged = default!;
        private void NotifyPropertyChanged([CallerMemberName] string caller = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        #endregion


        private readonly CatalogueContext _CatalogueDb;

        public string PageTitle => IsSearch ? "Suche" : "Items";

        private ObservableCollection<ItemWrapper> _Items = new ObservableCollection<ItemWrapper>();
        public ObservableCollection<ItemWrapper> Items
        {
            get { return _Items; }
            set
            {
                if (value != _Items)
                {
                    _Items = value;
                    NotifyPropertyChanged();
                }
            }
        }


        private bool _IsSearch = false;
        public bool IsSearch
        {
            get { return _IsSearch; }
            set
            {
                if (value != _IsSearch)
                {
                    _IsSearch = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(PageTitle));
                }
            }
        }

        private string _SearchText = "";
        public string SearchText
        {
            get { return _SearchText; }
            set
            {
                if (value != _SearchText)
                {
                    _SearchText = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _SearchCategoryId = -1;
        public int SearchCategoryId
        {
            get { return _SearchCategoryId; }
            set
            {
                if (value != _SearchCategoryId)
                {
                    _SearchCategoryId = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _SearchCategoryName = "Alle";
        public string SearchCategoryName
        {
            get { return _SearchCategoryName; }
            set
            {
                if (value != _SearchCategoryName)
                {
                    _SearchCategoryName = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public BrowseItemsViewModel()
        {
            _CatalogueDb = DependencyService.Resolve<CatalogueContext>();
        }


        public void SetIsSearch(bool isSearch)
        {
            IsSearch = isSearch;
        }

        public void SetSearchText(string searchText)
        {
            SearchText = searchText;
        }

        public void SetSearchCategoryId(int searchCategoryId)
        {
            SearchCategoryId = searchCategoryId;
        }

        public async Task LoadItems()
        {
            try
            {
                _Items.Clear();
                IQueryable<Item> itemQuery = _CatalogueDb.Items;

                if (IsSearch)
                {
                    if (SearchCategoryId != -1)
                    {
                        SearchCategoryName = (await _CatalogueDb.Categories.FindAsync(SearchCategoryId))?.Name ?? "-";
                        itemQuery = itemQuery.Where(i => i.CategoryId == SearchCategoryId);
                    }
                    if (!String.IsNullOrWhiteSpace(SearchText))
                        itemQuery = itemQuery.Where(i => i.Name.ToLower().Contains(SearchText)
                                                    || (i.Code != null && i.Code.ToLower().Contains(SearchText))
                                                    || (i.Category != null && i.Category.Name.ToLower().Contains(SearchText))
                                                    || (i.Description != null && i.Description.ToLower().Contains(SearchText))
                                                    || (i.Tags != null && i.Tags.ToLower().Contains(SearchText)));
                }

                List<ItemWrapper> items = await itemQuery.OrderBy(i => i.Name).Select(i => i.ToItemWrapper()).ToListAsync();
                foreach (ItemWrapper item in items)
                    _Items.Add(item);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error loading items: {e.Message}");
            }
        }

        public async Task DeleteItem(ItemWrapper item)
        {
            try
            {
                _CatalogueDb.Items.Remove(item.Item);
                await _CatalogueDb.SaveChangesAsync();
                Items.Remove(item);
                ImageManager.DeleteImage(item.Item.Name);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error deleting item: {e.Message}");
            }
        }
    }
}
