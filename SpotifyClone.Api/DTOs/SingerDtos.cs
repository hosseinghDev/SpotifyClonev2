using System.Collections.Generic;

namespace SpotifyClone.Api.DTOs
{
    public class SingerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public List<SongDto> Songs { get; set; }
        public List<CommentDto> Comments { get; set; }
    }
}