using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using BastelKatalog.Data;

namespace BastelKatalog.Models
{

    /// <summary>
    /// Wraps a Project object for UI usage
    /// </summary>
    public class ProjectWrapper : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged = default!;
        private void NotifyPropertyChanged([CallerMemberName] string caller = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        #endregion

        public Project Project { get; private set; }

        public string Name
        {
            get { return Project.Name; }
            set
            {
                if (Project.Name != value)
                {
                    Project.Name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string? Description
        {
            get { return Project.Description; }
            set
            {
                if (Project.Description != value)
                {
                    Project.Description = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ObservableCollection<ProjectItemWrapper> Items { get; private set; }


        public ProjectWrapper(Project project)
        {
            Project = project;
            Items = new ObservableCollection<ProjectItemWrapper>(project.Items.Select(i => i.ToProjectItemWrapper()));
        }
    }
}