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

        // --- NEW PROPERTIES FOR BUTTON ICONS ---
        public string ShuffleIcon => _globalAudioService.ShuffleIcon;
        public string RepeatIcon => _globalAudioService.RepeatIcon;

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

            // Notify UI of dependent property changes
            if (e.PropertyName is nameof(CurrentPosition)) OnPropertyChanged(nameof(PositionText));
            if (e.PropertyName is nameof(Duration)) OnPropertyChanged(nameof(DurationText));
            if (e.PropertyName is nameof(GlobalAudioService.IsShuffled)) OnPropertyChanged(nameof(ShuffleIcon));
            if (e.PropertyName is nameof(GlobalAudioService.RepeatMode)) OnPropertyChanged(nameof(RepeatIcon));
        }

        // --- All commands now simply call the service ---
        [RelayCommand]
        private void TogglePlayPause() => _globalAudioService.TogglePlayPause();

        [RelayCommand]
        private void Seek(double position) => _globalAudioService.Seek(position);

        [RelayCommand]
        private async Task SkipNext() => await _globalAudioService.SkipNextCommand.ExecuteAsync(null);

        [RelayCommand]
        private async Task SkipPrevious() => await _globalAudioService.SkipPreviousCommand.ExecuteAsync(null);

        [RelayCommand]
        private void ToggleShuffle() => _globalAudioService.ToggleShuffleCommand.Execute(null);

        [RelayCommand]
        private void ToggleRepeat() => _globalAudioService.ToggleRepeatCommand.Execute(null);


        public void Cleanup()
        {
            _globalAudioService.PropertyChanged -= GlobalAudioService_PropertyChanged;
        }
    }
}