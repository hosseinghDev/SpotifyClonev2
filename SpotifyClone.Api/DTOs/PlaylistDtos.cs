using System.Collections.Generic;

namespace SpotifyClone.Api.DTOs
{
    public class PlaylistDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string OwnerUsername { get; set; }
        public List<SongDto> Songs { get; set; }
    }

    public class CreatePlaylistDto
    {
        public string Name { get; set; }
    }

    public class AddSongToPlaylistDto
    {
        public int SongId { get; set; }
    }
}