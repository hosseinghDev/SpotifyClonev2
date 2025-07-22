using System.Collections.Generic;

namespace SpotifyClone.Maui.Models
{
    public class Singer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public List<Song> Songs { get; set; }
        public List<Comment> Comments { get; set; }
    }
}