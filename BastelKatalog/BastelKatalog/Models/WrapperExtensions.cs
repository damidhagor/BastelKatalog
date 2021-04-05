namespace BastelKatalog.Models
{
    /// <summary>
    /// Extensions for the Items and ItemWrappers
    /// </summary>
    public static class WrapperExtensions
    {
        /// <summary>
        /// Creates an ItemWrapper from an Item
        /// </summary>
        public static ItemWrapper ToItemWrapper(this Data.Item item)
        {
            ItemWrapper wrapper = new ItemWrapper(item);
            return wrapper;
        }

        /// <summary>
        /// Creates a ProjectWrapper from a Project
        /// </summary>
        public static ProjectWrapper ToProjectWrapper(this Data.Project project)
        {
            ProjectWrapper wrapper = new ProjectWrapper(project);
            return wrapper;
        }

        /// <summary>
        /// Creates a ProjectItemWrapper from a ProjectItem
        /// </summary>
        public static ProjectItemWrapper ToProjectItemWrapper(this Data.ProjectItem projectItem)
        {
            ProjectItemWrapper wrapper = new ProjectItemWrapper(projectItem);
            return wrapper;
        }
    }
}
