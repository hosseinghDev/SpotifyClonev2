using Plugin.Maui.Audio;
using System.Threading.Tasks;

namespace SpotifyClone.Maui.Services
{
    public class AudioPlayerService
    {
        private readonly IAudioManager _audioManager;
        private IAudioPlayer _currentPlayer;

        public AudioPlayerService(IAudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        public async Task PlayAudio(string url)
        {
            if (_currentPlayer != null)
            {
                _currentPlayer.Stop();
                _currentPlayer.Dispose();
            }

            _currentPlayer = _audioManager.CreatePlayer(url);
            _currentPlayer.Play();
        }

        public void StopAudio()
        {
            if (_currentPlayer != null && _currentPlayer.IsPlaying)
            {
                _currentPlayer.Stop();
            }
        }
    }
}