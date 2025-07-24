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

        // This property is now used for both displaying and setting the position
        [ObservableProperty]
        private double currentPosition;

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
            // Only update our copy of CurrentPosition if the service changes it
            if (e.PropertyName == nameof(_globalAudioService.CurrentPosition))
            {
                CurrentPosition = _globalAudioService.CurrentPosition;
            }

            OnPropertyChanged(e.PropertyName);
            if (e.PropertyName is nameof(CurrentPosition)) OnPropertyChanged(nameof(PositionText));
            if (e.PropertyName is nameof(Duration)) OnPropertyChanged(nameof(DurationText));
        }

        [RelayCommand]
        private void TogglePlayPause()
        {
            _globalAudioService.TogglePlayPause();
        }

        // CORRECTED SEEK COMMAND
        [RelayCommand]
        private void Seek()
        {
            // The Slider's Value is two-way bound to our CurrentPosition property.
            // When DragCompleted is fired, the value is already updated here.
            _globalAudioService.Seek(this.CurrentPosition);
        }

        public void Cleanup()
        {
            _globalAudioService.PropertyChanged -= GlobalAudioService_PropertyChanged;
        }
    }
}