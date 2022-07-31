using System.Threading.Tasks;
using Xamarin.Essentials;

namespace BastelKatalog.Helper
{
    internal static class PermissionHelper
    {
        public static async Task<bool> RequestStorageReadPermission()
        {
            var permissionStatus = await Permissions.CheckStatusAsync<Permissions.StorageRead>();

            if (permissionStatus != PermissionStatus.Granted)
                permissionStatus = await Permissions.RequestAsync<Permissions.StorageRead>();

            return permissionStatus == PermissionStatus.Granted;
        }

        public static async Task<bool> RequestStorageWritePermission()
        {
            var permissionStatus = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();

            if (permissionStatus != PermissionStatus.Granted)
                permissionStatus = await Permissions.RequestAsync<Permissions.StorageWrite>();

            return permissionStatus == PermissionStatus.Granted;
        }
    }
}
