using System.Text.Json.Serialization;

namespace BastelKatalog.Backup.Models
{
    public class Item
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Code { get; set; }

        public int? CategoryId { get; set; }

        public float Stock { get; set; }

        public string? Description { get; set; }

        public string? Tags { get; set; }

        public string? ImagePath { get; set; }

        [JsonConstructor]
        public Item(int id, string name, string? code, int? categoryId, float stock, string? description, string? tags, string? imagePath)
        {
            Id = id;
            Name = name;
            Code = code;
            CategoryId = categoryId;
            Stock = stock;
            Description = description;
            Tags = tags;
            ImagePath = imagePath;
        }

        public Item(Data.Item item)
        {
            Id = item.Id;
            Name = item.Name;
            Code = item.Code;
            CategoryId = item.CategoryId;
            Stock = item.Stock;
            Description = item.Description;
            Tags = item.Tags;
            ImagePath = item.ImagePath;
        }

        public Data.Item ToDataModel()
        {
            return new Data.Item(Name)
            {
                Id = Id,
                Code = Code,
                CategoryId = CategoryId,
                Stock = Stock,
                Description = Description,
                Tags = Tags,
                ImagePath = ImagePath
            };
        }
    }
}
