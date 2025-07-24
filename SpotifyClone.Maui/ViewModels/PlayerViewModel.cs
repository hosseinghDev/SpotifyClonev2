using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpotifyClone.Maui.Models;
using SpotifyClone.Maui.Services;
using System.ComponentModel;

namespace SpotifyClone.Maui.ViewModels
{
    public partial class PlayerViewModel : BaseViewModel
    {
        private readonly GlobalAudioService _globalAudioService;

        public Song? CurrentSong => _globalAudioService.CurrentSong;
        public double Duration => _globalAudioService.Duration;
        public double CurrentPosition => _globalAudioService.CurrentPosition;
        public bool IsPlaying => _globalAudioService.IsPlaying;
        public string PlayPauseButtonIcon => _globalAudioService.PlayPauseButtonIcon;

        public string PositionText => TimeSpan.FromSeconds(CurrentPosition).ToString(@"mm\:ss");
        public string DurationText => TimeSpan.FromSeconds(Duration).ToString(@"mm\:ss");

        public PlayerViewModel(GlobalAudioService globalAudioService)
        {
            _globalAudioService = globalAudioService;
            _globalAudioService.PropertyChanged += GlobalAudioService_PropertyChanged;
        }

        private void GlobalAudioService_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
            if (e.PropertyName is nameof(CurrentPosition)) OnPropertyChanged(nameof(PositionText));
            if (e.PropertyName is nameof(Duration)) OnPropertyChanged(nameof(DurationText));
        }

        [RelayCommand]
        private void TogglePlayPause()
        {
            _globalAudioService.TogglePlayPause();
        }

        // Final Seek Command - accepts a double
        [RelayCommand]
        private void Seek(double position)
        {
            _globalAudioService.Seek(position);
        }

        public void Cleanup()
        {
            _globalAudioService.PropertyChanged -= GlobalAudioService_PropertyChanged;
        }
    }
}