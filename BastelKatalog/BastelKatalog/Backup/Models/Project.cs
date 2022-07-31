namespace BastelKatalog.Backup.Models
{
    public class Project
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }


        public Project(BastelKatalog.Data.Project project)
        {
            Id = project.Id;
            Name = project.Name;
            Description = project.Description;
        }
    }
}
