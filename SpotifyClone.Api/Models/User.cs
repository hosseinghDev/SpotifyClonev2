using System.Collections.Generic;

namespace SpotifyClone.Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}