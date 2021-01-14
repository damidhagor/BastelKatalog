using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BastelKatalog.Data
{
    public static class ImageManager
    {
        public static string DEFAULT_IMAGE_NAME = "icon_image.png";

        public static string GetNewImageFilename() => $"{Guid.NewGuid()}.jpg";

        public static async Task<string?> SaveImage(byte[] image)
        {
            try
            {
                string filename = GetNewImageFilename();
                string path = GetImagePath(filename);
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                await File.WriteAllBytesAsync(path, image);
                return filename;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error saving image: {e.Message}");
                return null;
            }
        }

        public static ImageSource GetImage(string? name)
        {
            ImageSource? source = null;

            try
            {
                if (!String.IsNullOrWhiteSpace(name))
                {
                    string filename = GetImagePath(name);
                    if (File.Exists(filename))
                        source = ImageSource.FromFile(filename);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error getting image filename: {e.Message}");
            }

            return source ?? ImageSource.FromFile(DEFAULT_IMAGE_NAME);
        }

        public static void DeleteImage(string name)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(name))
                {
                    string filename = GetImagePath(name);
                    if (File.Exists(filename))
                        File.Delete(filename);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error getting image filename: {e.Message}");
            }
        }


        private static string GetImagePath(string filename)
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
                return Path.Combine(FileSystem.AppDataDirectory, "images", filename);
            else
                return Path.Combine("images", filename);
        }
    }
}
