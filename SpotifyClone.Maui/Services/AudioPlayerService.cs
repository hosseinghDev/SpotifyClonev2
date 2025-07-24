using Plugin.Maui.Audio;
using System.IO;
using System.Threading.Tasks;

namespace SpotifyClone.Maui.Services
{
    public class AudioPlayerService
    {
        private readonly IAudioManager _audioManager;
        private IAudioPlayer? _currentPlayer;

        public AudioPlayerService(IAudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        // Changed to accept a Stream instead of a URL
        public async Task PlayAudio(Stream audioStream)
        {
            if (_currentPlayer != null)
            {
                _currentPlayer.Stop();
                _currentPlayer.Dispose();
            }

            if (audioStream == null || audioStream.Length == 0)
            {
                return; // Do nothing if the stream is invalid
            }

            _currentPlayer = _audioManager.CreatePlayer(audioStream);
            _currentPlayer.Play();
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
        }
    }
}