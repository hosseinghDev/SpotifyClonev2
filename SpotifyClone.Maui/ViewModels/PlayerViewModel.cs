using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpotifyClone.Maui.Models;
using SpotifyClone.Maui.Services;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

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

                // Set duration for the slider
                Duration = _audioPlayerService.GetDuration();
                DurationText = TimeSpan.FromSeconds(Duration).ToString(@"mm\:ss");

                // Start a timer to update the current position
                _timer = Application.Current!.Dispatcher.CreateTimer();
                _timer.Interval = TimeSpan.FromMilliseconds(200);
                _timer.Tick += (s, e) =>
                {
                    CurrentPosition = _audioPlayerService.GetCurrentPosition();
                    PositionText = TimeSpan.FromSeconds(CurrentPosition).ToString(@"mm\:ss");
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
        // In PlayerViewModel.cs
        private void Seek()
        {
            // The Slider's Value is already bound to CurrentPosition,
            // so we just use that property's current value.
            _audioPlayerService.Seek(CurrentPosition);
        }

        public void StopAudio()
        {
            _timer?.Stop();
            _audioPlayerService.StopAudio();
        }
    }
}