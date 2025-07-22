namespace SpotifyClone.Api.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string? Text { get; set; }
        public string? Username { get; set; }
        public DateTime PostedAt { get; set; }
    }

    public class CreateCommentDto
    {
        public string? Text { get; set; }
    }
}