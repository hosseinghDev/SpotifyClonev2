using Plugin.Maui.Audio;
using System.IO;
using System.Threading.Tasks;

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
            if (_currentPlayer != null)
            {
                _currentPlayer.Stop();
                _currentPlayer.Dispose();
            }

            _currentStream = audioStream;
            if (_currentStream == null || _currentStream.Length == 0) return;

            _currentPlayer = _audioManager.CreatePlayer(_currentStream);
            _currentPlayer.Play();
        }

        public double GetDuration() => _currentPlayer?.Duration ?? 0;
        public double GetCurrentPosition() => _currentPlayer?.CurrentPosition ?? 0;

        public void Seek(double position)
        {
            if (_currentPlayer != null)
                _currentPlayer.Seek(position);
        }

        public void StopAudio()
        {
            if (_currentPlayer != null && _currentPlayer.IsPlaying)
            {
                _currentPlayer.Stop();
            }
        }

        public void Dispose()
        {
            if (_currentPlayer != null)
            {
                _currentPlayer.Stop();
                _currentPlayer.Dispose();
                _currentPlayer = null;
            }
            _currentStream?.Dispose();
            _currentStream = null;
        }
    }
}