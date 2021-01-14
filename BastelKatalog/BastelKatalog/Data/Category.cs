namespace BastelKatalog.Data
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }


        public Category(string name)
        {
            Name = name;
        }
    }
}
