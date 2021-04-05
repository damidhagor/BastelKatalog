using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BastelKatalog.Data;
using BastelKatalog.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Xamarin.Forms;

namespace BastelKatalog.ViewModels
{
    public class EditProjectViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged = default!;
        private void NotifyPropertyChanged([CallerMemberName] string caller = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        #endregion

        private readonly CatalogueContext _CatalogueDb;

        public bool WasSaved { get; private set; }

        private ProjectWrapper _Project;
        public ProjectWrapper Project
        {
            get { return _Project; }
            set
            {
                if (value != _Project)
                {
                    _Project = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public EditProjectViewModel()
        {
            _CatalogueDb = DependencyService.Resolve<CatalogueContext>();
            _Project = new ProjectWrapper(new Project(""));
        }


        public void LoadData(int projectId)
        {
            try
            {
                Project = _CatalogueDb.Projects.FirstOrDefault(p => p.Id == projectId)?.ToProjectWrapper()
                    ?? new ProjectWrapper(new Project(""));
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error loading project: {e.Message}");
            }
        }

        public void DeleteProjectItem(ProjectItemWrapper item)
        {
            try
            {
                _CatalogueDb.ProjectItems.Remove(item.ProjectItem);
                Project.Items.Remove(item);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error deleting project item: {e.Message}");
            }
        }

        public string? Validate()
        {
            // Name must be entered
            if (String.IsNullOrWhiteSpace(Project.Name))
                return "Bitte Namen angeben.";

            return null;
        }

        public async Task<bool> SaveProject()
        {
            try
            {
                _Project.Description = String.IsNullOrWhiteSpace(_Project.Description) ? null : _Project.Description;

                if (_Project.Project.Id == 0)
                    _CatalogueDb.Projects.Add(_Project.Project);
                await _CatalogueDb.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error saving project: {e.Message}");
                return false;
            }
        }

        public void RevertChanges()
        {
            EntityEntry<Project>? entry = _CatalogueDb.ChangeTracker.Entries<Project>().FirstOrDefault(i => i.Entity.Id == Project.Project.Id);
            if (entry?.State == EntityState.Modified)
            {
                entry.CurrentValues.SetValues(entry.OriginalValues);
            }

            List<EntityEntry<ProjectItem>> items = _CatalogueDb.ChangeTracker.Entries<ProjectItem>().Where(i => i.Entity.ProjectId == Project.Project.Id).ToList();
            foreach (EntityEntry<ProjectItem>? item in items)
            {
                switch (item.State)
                {
                    case EntityState.Modified:
                        item.CurrentValues.SetValues(item.OriginalValues);
                        item.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        item.State = EntityState.Detached;
                        break;
                    case EntityState.Deleted:
                        item.CurrentValues.SetValues(item.OriginalValues);
                        item.State = EntityState.Unchanged;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
