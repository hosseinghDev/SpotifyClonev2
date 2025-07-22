using System;

namespace SpotifyClone.Api.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime PostedAt { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        // A comment can be on a song OR a singer
        public int? SongId { get; set; }
        public Song Song { get; set; }

        public int? SingerId { get; set; }
        public Singer Singer { get; set; }
    }
}