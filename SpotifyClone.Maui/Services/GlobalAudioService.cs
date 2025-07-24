using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Maui.Audio;
using SpotifyClone.Maui.Models;
using SpotifyClone.Maui.Views;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;

namespace SpotifyClone.Maui.Services
{
    public enum RepeatMode { None, All, One }

    public partial class GlobalAudioService : ObservableObject
    {
        private readonly IAudioManager _audioManager;
        private readonly HttpClient _httpClient;
        private IAudioPlayer? _currentPlayer;
        private Stream? _currentStream;
        private IDispatcherTimer? _timer;

        private List<Song> _originalQueue = new();
        private ObservableCollection<Song> _playbackQueue = new();
        private int _currentQueueIndex = -1;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsVisible))]
        private Song? currentSong;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PlayPauseButtonIcon))]
        private bool isPlaying;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PlaybackProgress))]
        private double currentPosition;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PlaybackProgress))]
        private double duration;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ShuffleIcon))]
        private bool isShuffled;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RepeatIcon))]
        private RepeatMode repeatMode;

        public bool IsVisible => CurrentSong != null;
        public string PlayPauseButtonIcon => IsPlaying ? "pause_circle.png" : "play_circle.png";
        public double PlaybackProgress => (Duration > 0) ? CurrentPosition / Duration : 0;
        public string ShuffleIcon => IsShuffled ? "shuffle_on.png" : "shuffle.png";
        public string RepeatIcon
        {
            get
            {
                return RepeatMode switch
                {
                    RepeatMode.One => "repeat_one.png",
                    RepeatMode.All => "repeat.png", // Use the highlighted version for 'All'
                    _ => "repeat.png", // Use the standard version for 'None'
                };
            }
        }

        public GlobalAudioService(IAudioManager audioManager)
        {
            _audioManager = audioManager;
            _httpClient = new HttpClient();
        }

        public async Task StartPlayback(Song songToPlay, List<Song> songQueue)
        {
            _originalQueue = new List<Song>(songQueue);
            _playbackQueue = new ObservableCollection<Song>(songQueue);

            if (IsShuffled)
            {
                ShuffleQueue(songToPlay);
            }

            _currentQueueIndex = _playbackQueue.IndexOf(songToPlay);

            await PlayCurrentSongInQueue();
            await GoToPlayerPage();
        }

        private async Task PlayCurrentSongInQueue()
        {
            Stop();

            if (_currentQueueIndex < 0 || _currentQueueIndex >= _playbackQueue.Count)
            {
                // If index is out of bounds, stop playback
                CurrentSong = null;
                return;
            }

            CurrentSong = _playbackQueue[_currentQueueIndex];

            try
            {
                using var networkStream = await _httpClient.GetStreamAsync(CurrentSong.FileUrl);
                var memoryStream = new MemoryStream();
                await networkStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                _currentStream = memoryStream;

                _currentPlayer = _audioManager.CreatePlayer(_currentStream);
                _currentPlayer.PlaybackEnded += OnPlaybackEnded;
                _currentPlayer.Play();

                Duration = _currentPlayer.Duration;

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
                _timer.Start();
            }
            catch (Exception) { Stop(); }
        }

        private async void OnPlaybackEnded(object? sender, EventArgs e)
        {
            if (RepeatMode == RepeatMode.One)
            {
                await PlayCurrentSongInQueue();
            }
            else
            {
                await SkipNext();
            }
        }

        [RelayCommand]
        public void TogglePlayPause()
        {
            if (_currentPlayer == null) return;
            if (_currentPlayer.IsPlaying) _currentPlayer.Pause(); else _currentPlayer.Play();
            IsPlaying = _currentPlayer.IsPlaying;
        }

        public void Seek(double position) => _currentPlayer?.Seek(position);

        [RelayCommand]
        async Task GoToPlayerPage() => await Shell.Current.GoToAsync(nameof(PlayerPage));

        public void Stop()
        {
            _timer?.Stop();
            _timer = null;
            if (_currentPlayer != null)
            {
                _currentPlayer.PlaybackEnded -= OnPlaybackEnded;
                if (_currentPlayer.IsPlaying) _currentPlayer.Stop();
                _currentPlayer.Dispose();
                _currentPlayer = null;
            }
            _currentStream?.Dispose();
            _currentStream = null;
            IsPlaying = false;
            // Don't null out CurrentSong here, let the new track replace it
        }

        // --- NEW COMMANDS ---
        [RelayCommand]
        private async Task SkipNext()
        {
            if (_playbackQueue.Count == 0) return;

            _currentQueueIndex++;
            if (_currentQueueIndex >= _playbackQueue.Count)
            {
                if (RepeatMode == RepeatMode.All)
                {
                    _currentQueueIndex = 0; // Loop back to the start
                }
                else
                {
                    // End of playlist, stop playback
                    Stop();
                    CurrentSong = null; // Hide the player
                    return;
                }
            }
            await PlayCurrentSongInQueue();
        }

        [RelayCommand]
        private async Task SkipPrevious()
        {
            if (_playbackQueue.Count == 0) return;

            // If more than 3 seconds in, restart the current song
            if (CurrentPosition > 3)
            {
                Seek(0);
                return;
            }

            _currentQueueIndex--;
            if (_currentQueueIndex < 0)
            {
                _currentQueueIndex = 0; // Don't go past the beginning
            }
            await PlayCurrentSongInQueue();
        }

        [RelayCommand]
        private void ToggleShuffle()
        {
            IsShuffled = !IsShuffled;
            if (CurrentSong is null) return;

            if (IsShuffled)
            {
                ShuffleQueue(CurrentSong);
            }
            else
            {
                // Restore original order
                _playbackQueue = new ObservableCollection<Song>(_originalQueue);
            }
            _currentQueueIndex = _playbackQueue.IndexOf(CurrentSong);
        }

        private void ShuffleQueue(Song firstSong)
        {
            var shuffled = _originalQueue.OrderBy(x => Guid.NewGuid()).ToList();
            shuffled.Remove(firstSong);
            shuffled.Insert(0, firstSong);
            _playbackQueue = new ObservableCollection<Song>(shuffled);
        }

        [RelayCommand]
        private void ToggleRepeat()
        {
            RepeatMode = RepeatMode switch
            {
                RepeatMode.None => RepeatMode.All,
                RepeatMode.All => RepeatMode.One,
                RepeatMode.One => RepeatMode.None,
                _ => RepeatMode.None
            };
        }
    }
}