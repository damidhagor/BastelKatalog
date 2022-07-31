namespace BastelKatalog.Backup.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? ParentCategoryId { get; set; }

        public Category(BastelKatalog.Data.Category category)
        {
            Id = category.Id;
            Name = category.Name;
            ParentCategoryId = category.ParentCategoryId;
        }
    }
}
