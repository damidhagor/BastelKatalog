namespace BastelKatalog.Data
{
    public class Item
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Code { get; set; }

        public int? CategoryId { get; set; }

        public virtual Category? Category { get; set; }

        public float Stock { get; set; }

        public string? Description { get; set; }

        public string? Tags { get; set; }

        /// <summary>
        /// Paths of images relative to the main image folder seperated by a ':'
        /// </summary>
        public string? ImagePath { get; set; }


        public Item(string name)
        {
            Name = name;
        }
    }
}
