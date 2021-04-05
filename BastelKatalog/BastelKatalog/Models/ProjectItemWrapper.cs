using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BastelKatalog.Data;

namespace BastelKatalog.Models
{
    /// <summary>
    /// Wraps ProjectItem object for UI usage
    /// </summary>
    public class ProjectItemWrapper : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged = default!;
        private void NotifyPropertyChanged([CallerMemberName] string caller = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        #endregion

        public ProjectItem ProjectItem { get; private set; }

        public ItemWrapper Item { get; private set; }

        public float NeededStock
        {
            get { return ProjectItem.NeededStock; }
            set
            {
                if (ProjectItem.NeededStock != value)
                {
                    ProjectItem.NeededStock = Math.Max(0, value);
                    NotifyPropertyChanged();
                }
            }
        }


        public ProjectItemWrapper(ProjectItem item)
        {
            ProjectItem = item;
            Item = item.Item.ToItemWrapper();
        }
    }
}