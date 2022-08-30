using System.Text.Json.Serialization;

namespace BastelKatalog.Backup.Models
{
    public class ProjectItem
    {
        public int Id { get; set; }

        public int ItemId { get; set; }

        public int ProjectId { get; set; }

        public float NeededStock { get; set; }

        [JsonConstructor]
        public ProjectItem(int id, int itemId, int projectId, float neededStock)
        {
            Id = id;
            ItemId = itemId;
            ProjectId = projectId;
            NeededStock = neededStock;
        }

        public ProjectItem(Data.ProjectItem projectItem)
        {
            Id = projectItem.Id;
            ItemId = projectItem.ItemId;
            ProjectId = projectItem.ProjectId;
            NeededStock = projectItem.NeededStock;
        }

        public Data.ProjectItem ToDataModel()
        {
            return new Data.ProjectItem(Id, ItemId, ProjectId, NeededStock);
        }
    }
}
