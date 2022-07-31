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


        public Item(BastelKatalog.Data.Item item)
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
    }
}
