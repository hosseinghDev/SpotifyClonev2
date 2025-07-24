using Plugin.Maui.Audio;
using System.IO;

namespace SpotifyClone.Maui.Services
{
    public class AudioPlayerService
    {
        private readonly IAudioManager _audioManager;
        private IAudioPlayer? _currentPlayer;
        private Stream? _currentStream;

        public AudioPlayerService(IAudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        public async Task PlayAudio(Stream audioStream)
        {
            StopAudio(); // Stop any previous track

            _currentStream = audioStream;
            if (_currentStream == null || _currentStream.Length == 0) return;

            _currentPlayer = _audioManager.CreatePlayer(_currentStream);
            _currentPlayer.Play();
        }

        // NEW Methods for Play/Pause
        public void Play() => _currentPlayer?.Play();
        public void Pause() => _currentPlayer?.Pause();
        public bool IsPlaying => _currentPlayer?.IsPlaying ?? false;


        public double GetDuration() => _currentPlayer?.Duration ?? 0;
        public double GetCurrentPosition() => _currentPlayer?.CurrentPosition ?? 0;

        public void Seek(double position)
        {
            if (_currentPlayer != null)
                _currentPlayer.Seek(position);
        }

        public void StopAudio()
        {
            if (_currentPlayer != null)
            {
                if (_currentPlayer.IsPlaying)
                {
                    _currentPlayer.Stop();
                }
                // Dispose of the player and stream to free resources
                _currentPlayer.Dispose();
                _currentPlayer = null;
                _currentStream?.Dispose();
                _currentStream = null;
            }
        }
    }
}