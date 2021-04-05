using System.Collections.Generic;

namespace BastelKatalog.Data
{
    public class Project
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public virtual ICollection<ProjectItem> Items { get; set; }


        public Project(string name)
        {
            Name = name;
            Items = new List<ProjectItem>();
        }
    }
}
