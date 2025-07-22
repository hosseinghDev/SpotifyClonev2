namespace SpotifyClone.Api.Models
{
    public class Singer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public ICollection<Song> Songs { get; set; } = new List<Song>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}