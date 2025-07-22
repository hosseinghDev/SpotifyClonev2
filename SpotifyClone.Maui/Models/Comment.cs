using System;

namespace SpotifyClone.Maui.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Username { get; set; }
        public DateTime PostedAt { get; set; }
    }
}