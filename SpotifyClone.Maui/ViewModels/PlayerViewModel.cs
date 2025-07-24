using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpotifyClone.Maui.Models;
using SpotifyClone.Maui.Services;
using System.IO;
using System.Net.Http;

namespace SpotifyClone.Maui.ViewModels
{
    public partial class PlayerViewModel : BaseViewModel
    {
        private readonly AudioPlayerService _audioPlayerService;
        private readonly HttpClient _httpClient;
        private IDispatcherTimer? _timer;

        [ObservableProperty]
        Song? song;

        [ObservableProperty]
        double duration;

        [ObservableProperty]
        double currentPosition;

        [ObservableProperty]
        string positionText = "00:00";

        [ObservableProperty]
        string durationText = "00:00";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PlayPauseButtonIcon))]
        bool isPlaying;

        public string PlayPauseButtonIcon => IsPlaying ? "pause_circle.png" : "play_circle.png";

        public PlayerViewModel(Song song, AudioPlayerService audioPlayerService)
        {
            Title = "Now Playing";
            Song = song;
            _audioPlayerService = audioPlayerService;
            _httpClient = new HttpClient();
        }

        public async Task PlayAudio()
        {
            if (Song == null || string.IsNullOrEmpty(Song.FileUrl)) return;

            IsBusy = true;
            try
            {
                using var networkStream = await _httpClient.GetStreamAsync(Song.FileUrl);
                var memoryStream = new MemoryStream();
                await networkStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                await _audioPlayerService.PlayAudio(memoryStream);

                Duration = _audioPlayerService.GetDuration();
                DurationText = TimeSpan.FromSeconds(Duration).ToString(@"mm\:ss");

                _timer = Application.Current!.Dispatcher.CreateTimer();
                _timer.Interval = TimeSpan.FromMilliseconds(200);
                _timer.Tick += (s, e) =>
                {
                    CurrentPosition = _audioPlayerService.GetCurrentPosition();
                    PositionText = TimeSpan.FromSeconds(CurrentPosition).ToString(@"mm\:ss");
                    IsPlaying = _audioPlayerService.IsPlaying;
                };
                _timer.Start();
            }
            catch (System.Exception ex)
            {
                await Shell.Current.DisplayAlert("Playback Error", $"Could not load audio: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void Seek()
        {
            _audioPlayerService.Seek(CurrentPosition);
        }

        [RelayCommand]
        private void TogglePlayPause()
        {
            if (IsPlaying)
            {
                _audioPlayerService.Pause();
            }
            else
            {
                _audioPlayerService.Play();
            }
            IsPlaying = _audioPlayerService.IsPlaying;
        }

        public void StopAudio()
        {
            _timer?.Stop();
            _audioPlayerService.StopAudio();
        }
    }
}