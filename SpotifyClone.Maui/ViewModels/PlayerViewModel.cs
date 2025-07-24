using CommunityToolkit.Mvvm.ComponentModel;
using SpotifyClone.Maui.Models;
using SpotifyClone.Maui.Services;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpotifyClone.Maui.ViewModels
{
    public partial class PlayerViewModel : BaseViewModel
    {
        private readonly AudioPlayerService _audioPlayerService;
        private readonly HttpClient _httpClient; // Use a dedicated HttpClient for this

        [ObservableProperty]
        Song? song;

        // We now inject ApiService to get the base URL
        public PlayerViewModel(Song song, AudioPlayerService audioPlayerService)
        {
            Title = "Now Playing";
            Song = song;
            _audioPlayerService = audioPlayerService;

            // Create a simple HttpClient. It doesn't need the full ApiService.
            _httpClient = new HttpClient();
        }

        public async Task PlayAudio()
        {
            if (Song == null || string.IsNullOrEmpty(Song.FileUrl)) return;

            IsBusy = true;
            try
            {
                // Use our own HttpClient to download the song stream
                var audioStream = await _httpClient.GetStreamAsync(Song.FileUrl);

                // Pass the downloaded stream to the audio player
                await _audioPlayerService.PlayAudio(audioStream);
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

        public void StopAudio() => _audioPlayerService.StopAudio();
    }
}