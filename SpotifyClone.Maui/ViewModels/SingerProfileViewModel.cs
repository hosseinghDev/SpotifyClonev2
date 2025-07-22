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
        private readonly AudioPlayerService _audioPlayerService;

        [ObservableProperty] int singerId;
        [ObservableProperty] Singer singer;

        public SingerProfileViewModel(ApiService apiService, AudioPlayerService audioPlayerService)
        {
            _apiService = apiService;
            _audioPlayerService = audioPlayerService;
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

        [RelayCommand]
        async Task PlaySong(Song song)
        {
            if (song == null) return;
            await Shell.Current.Navigation.PushAsync(new PlayerPage(new PlayerViewModel(song, _audioPlayerService)));
        }

        [RelayCommand]
        async Task GoToSingerComments(int singerId) => await Shell.Current.GoToAsync($"{nameof(SingerCommentsPage)}?singerId={singerId}");
    }
}