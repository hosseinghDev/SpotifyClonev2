using CommunityToolkit.Mvvm.ComponentModel;
using SpotifyClone.Maui.Models;
using SpotifyClone.Maui.Services;

namespace SpotifyClone.Maui.ViewModels
{
    public partial class PlayerViewModel : BaseViewModel
    {
        private readonly AudioPlayerService _audioPlayerService;

        [ObservableProperty] Song song;

        public PlayerViewModel(Song song, AudioPlayerService audioPlayerService)
        {
            Title = "Now Playing";
            Song = song;
            _audioPlayerService = audioPlayerService;
            PlayAudio();
        }

        private async void PlayAudio()
        {
            if (Song != null && !string.IsNullOrEmpty(Song.FileUrl))
                await _audioPlayerService.PlayAudio(Song.FileUrl);
        }

        public void StopAudio() => _audioPlayerService.StopAudio();
    }
}