namespace SpotifyClone.Maui.Models
{
    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string SingerName { get; set; }
        public int SingerId { get; set; }
        public string Genre { get; set; }
        public string FileUrl { get; set; }
        public string ImageUrl { get; set; }
    }
}