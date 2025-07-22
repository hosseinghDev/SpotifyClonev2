namespace SpotifyClone.Api.Models
{
    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public int SingerId { get; set; }
        public Singer Singer { get; set; } = null!;
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();
    }
}