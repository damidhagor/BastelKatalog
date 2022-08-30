using System.Text.Json.Serialization;

namespace BastelKatalog.Backup.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? ParentCategoryId { get; set; }

        [JsonConstructor]
        public Category(string name)
        {
            Name = name;
        }

        public Category(Data.Category category)
        {
            Id = category.Id;
            Name = category.Name;
            ParentCategoryId = category.ParentCategoryId;
        }

        public Data.Category ToDataModel()
        {
            return new Data.Category(Name)
            {
                Id = Id,
                ParentCategoryId = ParentCategoryId
            };
        }
    }
}
