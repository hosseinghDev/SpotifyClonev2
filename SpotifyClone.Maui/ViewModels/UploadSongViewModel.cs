using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpotifyClone.Maui.Services;

namespace SpotifyClone.Maui.ViewModels
{
    public partial class UploadSongViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        [ObservableProperty] string titleText;
        [ObservableProperty] string singerName;
        [ObservableProperty] string genre;
        [ObservableProperty] string audioFileName;
        [ObservableProperty] string imageFileName;

        private FileResult _audioFile;
        private FileResult _imageFile;

        public UploadSongViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "Upload Song";
        }

        [RelayCommand]
        async Task SelectAudioFile()
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select an audio file",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "audio/mpeg" } }
                })
            });

            if (result != null) { _audioFile = result; AudioFileName = result.FileName; }
        }

        [RelayCommand]
        async Task SelectImageFile()
        {
            var result = await FilePicker.PickAsync(PickOptions.Images);
            if (result != null) { _imageFile = result; ImageFileName = result.FileName; }
        }

        [RelayCommand]
        async Task UploadSong()
        {
            if (IsBusy || string.IsNullOrWhiteSpace(TitleText) ||
                string.IsNullOrWhiteSpace(SingerName) || _audioFile == null || _imageFile == null)
            {
                await Shell.Current.DisplayAlert("Error", "Please fill all fields and select files.", "OK");
                return;
            }
            IsBusy = true;
            try
            {
                using var content = new MultipartFormDataContent();
                content.Add(new StringContent(TitleText), "Title");
                content.Add(new StringContent(SingerName), "SingerName");
                content.Add(new StringContent(Genre), "Genre");
                content.Add(new StreamContent(await _audioFile.OpenReadAsync()), "AudioFile", _audioFile.FileName);
                content.Add(new StreamContent(await _imageFile.OpenReadAsync()), "ImageFile", _imageFile.FileName);

                await _apiService.SetAuthToken();
                if (await _apiService.PostMultipartAsync("api/songs/upload", content))
                {
                    await Shell.Current.DisplayAlert("Success", "Song uploaded!", "OK");
                    TitleText = SingerName = Genre = AudioFileName = ImageFileName = string.Empty;
                    _audioFile = _imageFile = null;
                }
                else await Shell.Current.DisplayAlert("Error", "Upload failed.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}