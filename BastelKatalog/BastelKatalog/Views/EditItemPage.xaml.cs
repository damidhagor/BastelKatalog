using System;
using System.Diagnostics;
using System.IO;
using BastelKatalog.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BastelKatalog.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public partial class EditItemPage : ContentPage
    {
        public EditItemViewModel ViewModel { get; set; }

        private int _ItemId;
        public string ItemId
        {
            get { return _ItemId.ToString(); }
            set
            {
                if (Int32.TryParse(value, out int itemId) && itemId != _ItemId)
                    _ItemId = itemId;
                else
                    _ItemId = -1;
            }
        }


        public EditItemPage()
        {
            InitializeComponent();

            BindingContext = ViewModel = new EditItemViewModel();
        }

        private void OnAppearing(object sender, EventArgs e)
        {
            ViewModel.LoadData(_ItemId);
        }

        private void OnDisappearing(object sender, EventArgs e)
        {
            if (!ViewModel.WasSaved)
                ViewModel.RevertChanges();
        }


        private async void Image_Clicked(object sender, EventArgs e)
        {
            try
            {
                Plugin.Media.Abstractions.StoreCameraMediaOptions options = new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    AllowCropping = true,
                    Name = Guid.NewGuid().ToString()
                };

                // Get photo from camera
                Plugin.Media.Abstractions.MediaFile? photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(options);
                if (!String.IsNullOrWhiteSpace(photo?.Path) && File.Exists(photo.Path))
                {
                    byte[] data = await File.ReadAllBytesAsync(photo.Path);
                    ViewModel.SetImageData(data);

                    File.Delete(photo.Path);
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine($"Error taking picture: {exc.Message}");
            }
        }

        private void AddStock_Clicked(object sender, EventArgs e)
        {
            ViewModel.Item.Stock++;
        }

        private void MinusStock_Clicked(object sender, EventArgs e)
        {
            ViewModel.Item.Stock--;
        }

        private async void Save_Clicked(object sender, EventArgs e)
        {
            // Validate before save
            string? result = ViewModel.Validate();
            if (!String.IsNullOrWhiteSpace(result))
            {
                await DisplayAlert("Item kann nicht gespeichert werden", result, "Ok");
                return;
            }

            // Save
            bool saved = await ViewModel.SaveItem();

            // Ask if new item should be added or return to item list
            if (saved)
            {
                if (_ItemId < 1)
                {
                    if ("Nächstes hinzufügen" == await DisplayActionSheet($"Item erfolgreich gespeichert.", null, null, "Nächstes hinzufügen", "Fertig"))
                        ViewModel.Reset();
                    else
                        await AppShell.Current.GoToAsync("..");
                }
                else
                {
                    await DisplayAlert("Item erfolgreich gespeichert.", null, "Ok");
                    await AppShell.Current.GoToAsync("..");
                }
            }
            else
            {
                await DisplayAlert(null, "Das Item konnte nicht gespeichert werden.", "Ok");
                await AppShell.Current.GoToAsync("..");
            }
        }
    }
}