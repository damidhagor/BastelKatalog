using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BastelKatalog.Models;
using Microsoft.EntityFrameworkCore;
using Xamarin.Forms;

namespace BastelKatalog.ViewModels
{
    public class ShowProjectViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged = default!;
        private void NotifyPropertyChanged([CallerMemberName] string caller = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        #endregion

        private readonly Data.CatalogueContext _CatalogueDb;

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


        public ShowProjectViewModel()
        {
            _CatalogueDb = DependencyService.Resolve<Data.CatalogueContext>();
            _Project = new ProjectWrapper(new Data.Project(""));
        }


        public void LoadData(int projectId)
        {
            try
            {
                Data.Project p = _CatalogueDb.Projects.Find(projectId);
                var items = _CatalogueDb.ProjectItems.ToList();

                Project = _CatalogueDb.Projects.Include(p=>p.Items).FirstOrDefault(p => p.Id == projectId)?.ToProjectWrapper()
                    ?? new ProjectWrapper(new Data.Project(""));
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error loading project: {e.Message}");
            }
        }

        public async Task DeleteProjectItem(ProjectItemWrapper item)
        {
            try
            {
                _CatalogueDb.ProjectItems.Remove(item.ProjectItem);
                await _CatalogueDb.SaveChangesAsync();
                Project.Items.Remove(item);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error deleting project item: {e.Message}");
            }
        }
    }
}
