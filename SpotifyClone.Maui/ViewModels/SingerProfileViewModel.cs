using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpotifyClone.Maui.Models;
using SpotifyClone.Maui.Services;
using SpotifyClone.Maui.Views;

namespace SpotifyClone.Maui.ViewModels
{
    [QueryProperty(nameof(SingerId), "singerId")]
    public partial class SingerProfileViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;
        private readonly GlobalAudioService _globalAudioService; // <-- CHANGE to GlobalAudioService

        [ObservableProperty] int singerId;
        [ObservableProperty] Singer? singer;

        public SingerProfileViewModel(ApiService apiService, GlobalAudioService globalAudioService) // <-- INJECT GlobalAudioService
        {
            _apiService = apiService;
            _globalAudioService = globalAudioService; // <-- INITIALIZE
        }

        public async Task LoadSingerDetails()
        {
            if (IsBusy || SingerId == 0) return;
            IsBusy = true;
            try
            {
                var result = await _apiService.GetAsync<Singer>($"api/singers/{SingerId}");
                if (result != null)
                {
                    Singer = result;
                    Title = Singer.Name;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Could not load singer: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // THIS COMMAND IS NOW DIFFERENT
        [RelayCommand]
        async Task PlaySong(Song song)
        {
            if (song == null) return;
            await _globalAudioService.PlaySong(song);
        }

        [RelayCommand]
        async Task GoToSingerComments(int singerId) => await Shell.Current.GoToAsync($"{nameof(SingerCommentsPage)}?singerId={singerId}");
    }
}