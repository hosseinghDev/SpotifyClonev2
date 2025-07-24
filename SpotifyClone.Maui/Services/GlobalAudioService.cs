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
                    RepeatMode.All => "repeat_on.png", // Use the highlighted icon for 'All'
                    _ => "repeat.png", // Use the standard icon for 'None'
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

            if (IsShuffled)
            {
                ShuffleQueue(songToPlay);
            }
            else
            {
                _playbackQueue = new ObservableCollection<Song>(_originalQueue);
            }

            _currentQueueIndex = _playbackQueue.IndexOf(songToPlay);

            await PlayCurrentSongInQueue();
            await GoToPlayerPage();
        }

        private async Task PlayCurrentSongInQueue()
        {
            StopCurrentPlayer();

            if (_currentQueueIndex < 0 || _currentQueueIndex >= _playbackQueue.Count)
            {
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

                StartTimer();
            }
            catch (Exception) { Stop(); }
        }

        private void OnPlaybackEnded(object? sender, EventArgs e)
        {
            if (RepeatMode == RepeatMode.One)
            {
                MainThread.BeginInvokeOnMainThread(async () => await PlayCurrentSongInQueue());
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(async () => await SkipNext());
            }
        }

        private void StartTimer()
        {
            _timer?.Stop();
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

        private void StopCurrentPlayer()
        {
            _timer?.Stop();
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
        }

        public void Stop()
        {
            StopCurrentPlayer();
            CurrentSong = null;
            _originalQueue.Clear();
            _playbackQueue.Clear();
            _currentQueueIndex = -1;
        }

        [RelayCommand]
        private async Task SkipNext()
        {
            if (_playbackQueue.Count == 0) return;

            _currentQueueIndex++;
            if (_currentQueueIndex >= _playbackQueue.Count)
            {
                if (RepeatMode == RepeatMode.All)
                {
                    _currentQueueIndex = 0;
                }
                else
                {
                    Stop();
                    return;
                }
            }
            await PlayCurrentSongInQueue();
        }

        [RelayCommand]
        private async Task SkipPrevious()
        {
            if (_playbackQueue.Count == 0) return;

            if (CurrentPosition > 3)
            {
                Seek(0);
                return;
            }

            _currentQueueIndex--;
            if (_currentQueueIndex < 0)
            {
                _currentQueueIndex = 0;
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
                // Shuffle the original queue, keeping the current song at the top
                ShuffleQueue(CurrentSong);
            }
            else
            {
                // Restore the original order
                _playbackQueue = new ObservableCollection<Song>(_originalQueue);
            }
            // Find the new index of the current song
            _currentQueueIndex = _playbackQueue.IndexOf(CurrentSong);
        }

        private void ShuffleQueue(Song firstSong)
        {
            var random = new Random();
            var shuffled = _originalQueue.OrderBy(x => random.Next()).ToList();
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