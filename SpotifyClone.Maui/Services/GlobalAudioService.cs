using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Maui.Audio;
using SpotifyClone.Maui.Models;
using SpotifyClone.Maui.Views;
using System.IO;
using System.Net.Http;

namespace SpotifyClone.Maui.Services
{
    public partial class GlobalAudioService : ObservableObject
    {
        private readonly IAudioManager _audioManager;
        private readonly HttpClient _httpClient;
        private IAudioPlayer? _currentPlayer;
        private Stream? _currentStream;
        private IDispatcherTimer? _timer;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsVisible))]
        private Song? currentSong;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PlayPauseButtonIcon))]
        private bool isPlaying;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PlaybackProgress))] // Notify progress changes
        private double currentPosition;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PlaybackProgress))] // Notify progress changes
        private double duration;

        public bool IsVisible => CurrentSong != null;
        public string PlayPauseButtonIcon => IsPlaying ? "pause_circle.png" : "play_circle.png";

        // NEW: Calculated property for the progress bar
        public double PlaybackProgress => (Duration > 0) ? CurrentPosition / Duration : 0;

        public GlobalAudioService(IAudioManager audioManager)
        {
            _audioManager = audioManager;
            _httpClient = new HttpClient();

            _timer = Application.Current!.Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(200);
            _timer.Tick += (s, e) =>
            {
                if (_currentPlayer != null)
                {
                    CurrentPosition = _currentPlayer.CurrentPosition;
                    IsPlaying = _currentPlayer.IsPlaying;
                }
            };
        }

        public async Task PlaySong(Song song)
        {
            if (CurrentSong?.Id == song.Id)
            {
                await GoToPlayerPage();
                return;
            }

            Stop();
            CurrentSong = song;

            try
            {
                using var networkStream = await _httpClient.GetStreamAsync(song.FileUrl);
                var memoryStream = new MemoryStream();
                await networkStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                _currentStream = memoryStream;
                _currentPlayer = _audioManager.CreatePlayer(_currentStream);

                _currentPlayer.Play();
                Duration = _currentPlayer.Duration;
                _timer?.Start();
                await GoToPlayerPage();
            }
            catch (Exception) { Stop(); }
        }

        [RelayCommand]
        public void TogglePlayPause()
        {
            if (_currentPlayer == null) return;
            if (_currentPlayer.IsPlaying) _currentPlayer.Pause(); else _currentPlayer.Play();
            IsPlaying = _currentPlayer.IsPlaying;
        }

        public void Seek(double position)
        {
            _currentPlayer?.Seek(position);
        }

        [RelayCommand]
        async Task GoToPlayerPage()
        {
            await Shell.Current.GoToAsync(nameof(PlayerPage));
        }

        public void Stop()
        {
            _timer?.Stop();
            if (_currentPlayer != null)
            {
                if (_currentPlayer.IsPlaying) _currentPlayer.Stop();
                _currentPlayer.Dispose();
                _currentPlayer = null;
            }
            _currentStream?.Dispose();
            _currentStream = null;
            CurrentSong = null;
            IsPlaying = false;
        }
    }
}