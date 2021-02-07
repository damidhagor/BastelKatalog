using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BastelKatalog.Data;

namespace BastelKatalog.Models
{
    /// <summary>
    /// Position of a category among its sibling categories
    /// </summary>
    public enum SubCategoryPosition
    {
        /// <summary>
        /// Default
        /// </summary>
        None,
        /// <summary>
        /// First category of many
        /// </summary>
        Start,
        /// <summary>
        /// Middle category of many
        /// </summary>
        Middle,
        /// <summary>
        /// Last category of many or single
        /// </summary>
        End
    }

    /// <summary>
    /// Wraps a Category object for UI usage
    /// </summary>
    public class CategoryWrapper : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged = default!;
        private void NotifyPropertyChanged([CallerMemberName] string caller = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        #endregion

        public Category Category { get; private set; }

        public string Name
        {
            get { return Category.Name; }
            set
            {
                if (Category.Name != value)
                {
                    Category.Name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private CategoryWrapper? _ParentCategory;
        public CategoryWrapper? ParentCategory
        {
            get { return _ParentCategory; }
            set
            {
                if (_ParentCategory != value)
                {
                    _ParentCategory = value;
                    if (Category.ParentCategoryId != value?.Category?.Id)
                    {
                        Category.ParentCategoryId = value?.Category?.Id;
                        Category.ParentCategory = value?.Category;
                    }
                    NotifyPropertyChanged();
                }
            }
        }

        public SubCategoryPosition SubCategoryPosition { get; set; }


        public CategoryWrapper(Category category)
        {
            Category = category;
            if (category.ParentCategory != null)
                _ParentCategory = new CategoryWrapper(category.ParentCategory);
            SubCategoryPosition = SubCategoryPosition.None;
        }
    }
}