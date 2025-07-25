using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SpotifyClone.Maui.Models;
using SpotifyClone.Maui.Services;
using SpotifyClone.Maui.Views;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;
using SpotifyClone.Maui.Messages;

namespace SpotifyClone.Maui.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;
        private readonly GlobalAudioService _globalAudioService;
        private readonly IMessenger _messenger;

        [ObservableProperty]
        ObservableCollection<Song> songs;

        [ObservableProperty]
        string? searchText;

        public HomeViewModel(ApiService apiService, GlobalAudioService globalAudioService, IMessenger messenger)
        {
            _apiService = apiService;
            _globalAudioService = globalAudioService;
            _messenger = messenger;
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
                await _apiService.SetAuthToken(); // <-- Ensure token is set before calling
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

        [RelayCommand]
        async Task PlaySong(Song song)
        {
            if (song == null) return;
            await _globalAudioService.StartPlayback(song, Songs.ToList());
        }

        
        [RelayCommand]
        async Task ToggleLike(Song song)
        {
            if (song is null) return;

            await _apiService.SetAuthToken();
            var result = await _apiService.PostAsync<LikeResult>($"api/user/songs/{song.Id}/like", new { });
            if (result != null)
            {
                song.IsLiked = result.IsLiked;
                _messenger.Send(new LibraryContentChangedMessage());
            }
        }

        [RelayCommand]
        async Task AddToPlaylist(Song song)
        {
            if (song is null) return;

            await _apiService.SetAuthToken();
            var playlists = await _apiService.GetAsync<List<Playlist>>("api/playlists");

            if (playlists is null || playlists.Count == 0)
            {
                await Shell.Current.DisplayAlert("No Playlists", "You don't have any playlists. Create one first!", "OK");
                return;
            }

            var playlistNames = playlists.Select(p => p.Name).ToArray();
            string chosenPlaylistName = await Shell.Current.DisplayActionSheet("Add to Playlist", "Cancel", null, playlistNames);

            if (!string.IsNullOrEmpty(chosenPlaylistName) && chosenPlaylistName != "Cancel")
            {
                var chosenPlaylist = playlists.FirstOrDefault(p => p.Name == chosenPlaylistName);
                if (chosenPlaylist != null)
                {
                    var success = await _apiService.PostAsync($"api/playlists/{chosenPlaylist.Id}/songs", new { SongId = song.Id });
                    if (success)
                    {
                        await Shell.Current.DisplayAlert("Success", $"Added '{song.Title}' to '{chosenPlaylist.Name}'.", "OK");
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "Failed to add song. It might already be in the playlist.", "OK");
                    }
                }
            }
        }
        

        [RelayCommand]
        async Task GoToSingerProfile(int singerId) => await Shell.Current.GoToAsync($"{nameof(SingerProfilePage)}?singerId={singerId}");

        [RelayCommand]
        async Task GoToComments(int songId) => await Shell.Current.GoToAsync($"{nameof(CommentsPage)}?songId={songId}");
        

        [RelayCommand]
        async Task Logout()
        {
            bool confirm = await Shell.Current.DisplayAlert("Log Out", "Are you sure you want to log out?", "Yes", "No");
            if (confirm)
            {
                // Stop any currently playing music
                _globalAudioService.Stop();

                // Clear the stored authentication token
                SecureStorage.Default.Remove("auth_token");

                // Set the main page of the application to the authentication shell
                Application.Current.MainPage = new AppShell_Auth();
            }
        }
    }

    // Helper class for deserializing the like result
    public class LikeResult
    {
        public bool IsLiked { get; set; }
    }
}