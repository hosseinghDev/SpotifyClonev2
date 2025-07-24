namespace SpotifyClone.Api.Models
{
    // This is a join table for the many-to-many relationship
    public class UserLikedSong
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int SongId { get; set; }
        public Song Song { get; set; } = null!;
    }
}