using Microsoft.AspNetCore.Http;
using System;

namespace SpotifyClone.Api.DTOs
{
    public class SongDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string SingerName { get; set; }
        public int SingerId { get; set; }
        public string Genre { get; set; }
        public string FileUrl { get; set; }
        public string ImageUrl { get; set; }
    }

    public class SongUploadDto
    {
        public string Title { get; set; }
        public string SingerName { get; set; }
        public string Genre { get; set; }
        public IFormFile AudioFile { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}