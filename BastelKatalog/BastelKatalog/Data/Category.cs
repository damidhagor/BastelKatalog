using System.Collections.Generic;

namespace BastelKatalog.Data
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? ParentCategoryId { get; set; }

        public Category? ParentCategory { get; set; }

        public ICollection<Category> SubCategories { get; set; }


        public Category(string name)
        {
            Name = name;
            SubCategories = new List<Category>();
        }
    }
}
