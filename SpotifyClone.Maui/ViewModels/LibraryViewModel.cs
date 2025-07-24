using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SpotifyClone.Maui.Messages;
using SpotifyClone.Maui.Models;
using SpotifyClone.Maui.Services;
using SpotifyClone.Maui.Views;
using System.Collections.ObjectModel;

namespace SpotifyClone.Maui.ViewModels
{
    public partial class LibraryViewModel : BaseViewModel, IRecipient<LibraryContentChangedMessage>
    {
        private readonly ApiService _apiService;
        private readonly GlobalAudioService _globalAudioService;

        [ObservableProperty]
        private ObservableCollection<Playlist> playlists;

        [ObservableProperty]
        private ObservableCollection<Song> likedSongs;

        public LibraryViewModel(ApiService apiService, GlobalAudioService globalAudioService, IMessenger messenger)
        {
            _apiService = apiService;
            _globalAudioService = globalAudioService;
            Title = "Library";
            Playlists = new ObservableCollection<Playlist>();
            LikedSongs = new ObservableCollection<Song>();

            // Register to receive messages
            messenger.Register(this);
        }

        // This is called when another part of the app (like Home page) changes the library content.
        public void Receive(LibraryContentChangedMessage message)
        {
            // We received a notification that liked songs have changed.
            // We can just reload the liked songs section.
            IsBusy = true; // Briefly show indicator
            Task.Run(async () =>
            {
                await LoadLikedSongsAsync();
                IsBusy = false;
            });
        }

        // This is called by OnAppearing to load everything.
        [RelayCommand]
        async Task LoadLibraryData()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                // Load both parts in parallel for speed
                await Task.WhenAll(LoadPlaylistsAsync(), LoadLikedSongsAsync());
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Could not load library: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadPlaylistsAsync()
        {
            await _apiService.SetAuthToken();
            var userPlaylists = await _apiService.GetAsync<List<Playlist>>("api/playlists");

            // All UI updates MUST happen on the main thread
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (userPlaylists != null)
                {
                    Playlists.Clear();
                    foreach (var playlist in userPlaylists)
                    {
                        Playlists.Add(playlist);
                    }
                }
            });
        }

        private async Task LoadLikedSongsAsync()
        {
            await _apiService.SetAuthToken();
            var userLikedSongs = await _apiService.GetAsync<List<Song>>("api/user/songs/liked");

            // All UI updates MUST happen on the main thread
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (userLikedSongs != null)
                {
                    LikedSongs.Clear();
                    foreach (var song in userLikedSongs)
                    {
                        LikedSongs.Add(song);
                    }
                }
            });
        }

        [RelayCommand]
        async Task CreatePlaylistAsync()
        {
            string name = await Shell.Current.DisplayPromptAsync("New Playlist", "Enter playlist name:");
            if (!string.IsNullOrWhiteSpace(name))
            {
                await _apiService.SetAuthToken();
                var result = await _apiService.PostAsync("api/playlists", new { Name = name });
                if (result) await LoadPlaylistsAsync(); // Just reload playlists
                else await Shell.Current.DisplayAlert("Error", "Could not create playlist.", "OK");
            }
        }

        [RelayCommand]
        async Task GoToPlaylistDetail(Playlist playlist)
        {
            if (playlist == null) return;
            await Shell.Current.GoToAsync($"{nameof(PlaylistDetailPage)}?playlistId={playlist.Id}");
        }

        [RelayCommand]
        async Task PlayLikedSong(Song song)
        {
            if (song == null) return;
            await _globalAudioService.StartPlayback(song, LikedSongs.ToList());
        }
    }
}