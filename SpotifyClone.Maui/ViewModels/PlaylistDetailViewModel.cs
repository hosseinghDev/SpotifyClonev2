using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpotifyClone.Maui.Models;
using SpotifyClone.Maui.Services;
using SpotifyClone.Maui.Views;
using System.Collections.ObjectModel;

namespace SpotifyClone.Maui.ViewModels
{
    [QueryProperty(nameof(PlaylistId), "playlistId")]
    public partial class PlaylistDetailViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;
        private readonly AudioPlayerService _audioPlayerService;

        [ObservableProperty] int playlistId;
        [ObservableProperty] Playlist playlist;
        [ObservableProperty] ObservableCollection<Song> songs;

        public PlaylistDetailViewModel(ApiService apiService, AudioPlayerService audioPlayerService)
        {
            _apiService = apiService;
            _audioPlayerService = audioPlayerService;
            Songs = new ObservableCollection<Song>();
        }

        public async Task LoadPlaylistDetails()
        {
            if (IsBusy || PlaylistId == 0) return;
            IsBusy = true;
            try
            {
                await _apiService.SetAuthToken();
                var result = await _apiService.GetAsync<Playlist>($"api/playlists/{PlaylistId}");
                if (result != null)
                {
                    Playlist = result;
                    Title = Playlist.Name;
                    Songs.Clear();
                    foreach (var song in result.Songs) Songs.Add(song);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Could not load details: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task RemoveSong(Song song)
        {
            if (song == null) return;
            bool confirm = await Shell.Current.DisplayAlert("Confirm", $"Remove '{song.Title}' from playlist?", "Yes", "No");
            if (confirm)
            {
                await _apiService.SetAuthToken();
                var success = await _apiService.DeleteAsync($"api/playlists/{PlaylistId}/songs/{song.Id}");
                if (success) Songs.Remove(song);
                else await Shell.Current.DisplayAlert("Error", "Failed to remove song.", "OK");
            }
        }

        [RelayCommand]
        async Task PlaySong(Song song)
        {
            if (song == null) return;
            await Shell.Current.Navigation.PushAsync(new PlayerPage(new PlayerViewModel(song, _audioPlayerService)));
        }
    }
}