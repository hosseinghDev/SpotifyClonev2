namespace SpotifyClone.Api.Models
{
    public class PlaylistSong
    {
        public int PlaylistId { get; set; }
        public Playlist Playlist { get; set; } = null!;
        public int SongId { get; set; }
        public Song Song { get; set; } = null!;
    }
}