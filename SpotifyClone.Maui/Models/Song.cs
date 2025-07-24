using CommunityToolkit.Mvvm.ComponentModel; // <-- ADD THIS

namespace SpotifyClone.Maui.Models
{
    public partial class Song : ObservableObject // <-- CHANGE THIS
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string SingerName { get; set; }
        public int SingerId { get; set; }
        public string Genre { get; set; }
        public string FileUrl { get; set; }
        public string ImageUrl { get; set; }

        [ObservableProperty] // <-- ADD THIS
        private bool isLiked;
    }
}