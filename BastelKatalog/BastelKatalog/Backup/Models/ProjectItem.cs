namespace BastelKatalog.Backup.Models
{
    public class ProjectItem
    {
        public int Id { get; set; }

        public int ItemId { get; set; }

        public int ProjectId { get; set; }

        public float NeededStock { get; set; }


        public ProjectItem(BastelKatalog.Data.ProjectItem projectItem)
        {
            Id = projectItem.Id;
            ItemId = projectItem.ItemId;
            ProjectId = projectItem.ProjectId;
            NeededStock = projectItem.NeededStock;
        }
    }
}
