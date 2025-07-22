using System.Collections.Generic;

namespace SpotifyClone.Maui.Models
{
    public class Playlist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string OwnerUsername { get; set; }
        public List<Song> Songs { get; set; } = new();
    }
}