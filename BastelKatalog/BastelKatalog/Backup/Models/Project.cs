using System.Text.Json.Serialization;

namespace BastelKatalog.Backup.Models
{
    public class Project
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        [JsonConstructor]
        public Project(int id, string name, string? description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public Project(Data.Project project)
        {
            Id = project.Id;
            Name = project.Name;
            Description = project.Description;
        }

        public Data.Project ToDataModel()
        {
            return new Data.Project(Name)
            {
                Id = Id,
                Description = Description
            };
        }
    }
}
