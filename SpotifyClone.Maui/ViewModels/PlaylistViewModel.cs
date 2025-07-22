using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpotifyClone.Maui.Models;
using SpotifyClone.Maui.Services;
using SpotifyClone.Maui.Views;
using System.Collections.ObjectModel;

namespace SpotifyClone.Maui.ViewModels
{
    public partial class PlaylistViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        [ObservableProperty] ObservableCollection<Playlist> playlists;

        public PlaylistViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "My Playlists";
            Playlists = new ObservableCollection<Playlist>();
        }

        [RelayCommand]
        async Task LoadPlaylistsAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                await _apiService.SetAuthToken();
                var userPlaylists = await _apiService.GetAsync<List<Playlist>>("api/playlists");
                if (userPlaylists != null)
                {
                    Playlists.Clear();
                    foreach (var playlist in userPlaylists) Playlists.Add(playlist);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Could not load playlists: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task CreatePlaylistAsync()
        {
            string name = await Shell.Current.DisplayPromptAsync("New Playlist", "Enter playlist name:");
            if (!string.IsNullOrWhiteSpace(name))
            {
                await _apiService.SetAuthToken();
                var result = await _apiService.PostAsync("api/playlists", new { Name = name });
                if (result) await LoadPlaylistsAsync();
                else await Shell.Current.DisplayAlert("Error", "Could not create playlist.", "OK");
            }
        }

        [RelayCommand]
        async Task GoToPlaylistDetail(Playlist playlist)
        {
            if (playlist == null) return;
            await Shell.Current.GoToAsync($"{nameof(PlaylistDetailPage)}?playlistId={playlist.Id}");
        }
    }
}