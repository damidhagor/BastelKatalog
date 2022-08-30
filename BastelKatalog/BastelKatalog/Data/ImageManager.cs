using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BastelKatalog.Data
{
    /// <summary>
    /// Manages image access for saving, retrieval and deleting
    /// </summary>
    public static class ImageManager
    {
        /// <summary>
        /// Default fallback image when no image is available
        /// </summary>
        public static string DEFAULT_IMAGE_NAME = "icon_image.png";

        /// <summary>
        /// Creates a new image name
        /// </summary>
        /// <returns></returns>
        public static string GetNewImageFilename() => $"{Guid.NewGuid()}.jpg";

        /// <summary>
        /// Saves an image to storage and returns the relative image path
        /// </summary>
        /// <param name="image"></param>
        /// <returns>Relative image path</returns>
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

        /// <summary>
        /// Gets an image from storage.
        /// If image is not foud returns default image.
        /// </summary>
        /// <param name="name">Relative image path</param>
        /// <returns>Loaded image</returns>
        public async static Task<byte[]?> GetImageAsync(string? name, CancellationToken cancellationToken)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(name))
                {
                    string filename = GetImagePath(name);
                    return File.Exists(filename)
                        ? await File.ReadAllBytesAsync(filename, cancellationToken)
                        : await File.ReadAllBytesAsync(DEFAULT_IMAGE_NAME, cancellationToken);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error getting image filename: {e.Message}");
            }

            return null;
        }

        /// <summary>
        /// Gets an image from storage.
        /// If image is not foud returns default image.
        /// </summary>
        /// <param name="name">Relative image path</param>
        /// <returns>Loaded image</returns>
        public static ImageSource GetImageAsImageSource(string? name)
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


        /// <summary>
        /// Deletes an image from storage.
        /// </summary>
        /// <param name="name">Relative file path</param>
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


        /// <summary>
        /// Exports all images by copying the images folder to a destination folder.
        /// </summary>
        public static async Task ExportImages(string destinationFolder, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                var exportDirectory = Path.Combine(destinationFolder, "images");
                Directory.CreateDirectory(exportDirectory);

                foreach (var file in Directory.GetFiles(GetImageDirectory()))
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    File.Copy(file, Path.Combine(exportDirectory, Path.GetFileName(file)));
                }
            });
        }

        /// <summary>
        /// Imports all images by deleting the existing images and copying the new images to the storage folder.
        /// </summary>
        public static async Task ImportImages(string sourceFolder, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                DeleteAllImages();

                var imagesDirectory = GetImageDirectory();

                Directory.CreateDirectory(imagesDirectory);

                foreach (var file in Directory.GetFiles(sourceFolder))
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    File.Copy(file, Path.Combine(imagesDirectory, Path.GetFileName(file)));
                }
            });
        }


        /// <summary>
        /// Deletes all saved images from storage.
        /// </summary>
        private static void DeleteAllImages()
        {
            if (Directory.Exists(GetImageDirectory()))
                Directory.Delete(GetImageDirectory(), true);
        }


        /// <summary>
        /// Gets the absolute image directory path.
        /// </summary>
        /// <returns>Absolute image directory path</returns>
        private static string GetImageDirectory()
            => DeviceInfo.Platform == DevicePlatform.Android
                ? Path.Combine(FileSystem.AppDataDirectory, "images")
                : "images";

        /// <summary>
        /// Gets the absolute image path for a relative image path
        /// </summary>
        /// <param name="filename">Relative image path</param>
        /// <returns>Absolute image path</returns>
        private static string GetImagePath(string filename)
            => Path.Combine(GetImageDirectory(), filename);
    }
}
