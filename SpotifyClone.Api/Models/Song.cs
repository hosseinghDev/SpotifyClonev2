using System;
using System.Collections.Generic;

namespace SpotifyClone.Api.Models
{
    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public string FilePath { get; set; } // Path on the server
        public string ImageUrl { get; set; }
        public DateTime UploadedAt { get; set; }

        public int SingerId { get; set; }
        public Singer Singer { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();
    }
}