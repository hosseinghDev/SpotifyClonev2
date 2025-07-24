using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpotifyClone.Maui.Models;
using SpotifyClone.Maui.Services;
using SpotifyClone.Maui.Views;
using System.Collections.ObjectModel;

namespace SpotifyClone.Maui.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;
        private readonly GlobalAudioService _globalAudioService; // <-- CHANGE to GlobalAudioService

        [ObservableProperty]
        ObservableCollection<Song> songs;

        [ObservableProperty]
        string? searchText;

        public HomeViewModel(ApiService apiService, GlobalAudioService globalAudioService) // <-- INJECT GlobalAudioService
        {
            _apiService = apiService;
            _globalAudioService = globalAudioService; // <-- INITIALIZE
            Title = "Home";
            Songs = new ObservableCollection<Song>();
        }

        [RelayCommand]
        async Task LoadSongsAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var songsList = await _apiService.GetAsync<List<Song>>($"api/songs?search={SearchText}");
                if (songsList != null)
                {
                    Songs.Clear();
                    foreach (var song in songsList) Songs.Add(song);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Could not load songs: {ex.Message}", "OK");
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
        async Task GoToSingerProfile(int singerId) => await Shell.Current.GoToAsync($"{nameof(SingerProfilePage)}?singerId={singerId}");

        [RelayCommand]
        async Task GoToComments(int songId) => await Shell.Current.GoToAsync($"{nameof(CommentsPage)}?songId={songId}");
    }
}