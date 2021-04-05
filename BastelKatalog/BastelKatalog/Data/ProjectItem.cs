using System.Collections.Generic;

namespace BastelKatalog.Data
{
    public class ProjectItem
    {
        public int Id { get; set; }

        public int ItemId { get; set; }

        public virtual Item Item { get; set; }

        public int ProjectId { get; set; }

        public virtual Project Project { get; set; }

        public float NeededStock { get; set; }


        protected ProjectItem() { }

        public ProjectItem(Project project, Item item)
        {
            ItemId = item.Id;
            Item = item;
            ProjectId = project.Id;
            Project = project;
        }
    }
}
