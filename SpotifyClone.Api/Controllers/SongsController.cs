using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpotifyClone.Api.Data;
using SpotifyClone.Api.DTOs;
using SpotifyClone.Api.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyClone.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SongsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public SongsController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        private string GetBaseUrl()
        {
            return $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SongDto>>> GetSongs([FromQuery] string search)
        {
            var query = _context.Songs.Include(s => s.Singer).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.Title.Contains(search) ||
                                         s.Singer.Name.Contains(search) ||
                                         s.Genre.Contains(search));
            }

            var songs = await query.ToListAsync();
            var baseUrl = GetBaseUrl();

            return songs.Select(s => new SongDto
            {
                Id = s.Id,
                Title = s.Title,
                SingerName = s.Singer.Name,
                SingerId = s.SingerId,
                Genre = s.Genre,
                FileUrl = $"{baseUrl}/api/songs/stream/{s.Id}",
                ImageUrl = string.IsNullOrEmpty(s.ImageUrl) ? null : $"{baseUrl}/{s.ImageUrl.Replace("\\", "/")}"
            }).ToList();
        }

        [HttpGet("stream/{id}")]
        public async Task<IActionResult> StreamSong(int id)
        {
            var song = await _context.Songs.FindAsync(id);
            if (song == null || string.IsNullOrEmpty(song.FilePath))
            {
                return NotFound();
            }

            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, song.FilePath);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found on server.");
            }

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(stream, "audio/mpeg", enableRangeProcessing: true);
        }


        [Authorize]
        [HttpPost("upload")]
        public async Task<ActionResult<SongDto>> UploadSong([FromForm] SongUploadDto uploadDto)
        {
            if (uploadDto.AudioFile == null || uploadDto.AudioFile.Length == 0)
                return BadRequest("No audio file uploaded.");
            if (uploadDto.ImageFile == null || uploadDto.ImageFile.Length == 0)
                return BadRequest("No image file uploaded.");

            var uploadsFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
            var songsFolderPath = Path.Combine(uploadsFolderPath, "songs");
            var imagesFolderPath = Path.Combine(uploadsFolderPath, "images");

            Directory.CreateDirectory(songsFolderPath);
            Directory.CreateDirectory(imagesFolderPath);

            // Save audio file
            var audioFileName = $"{Guid.NewGuid()}_{uploadDto.AudioFile.FileName}";
            var audioFilePath = Path.Combine(songsFolderPath, audioFileName);
            using (var stream = new FileStream(audioFilePath, FileMode.Create))
            {
                await uploadDto.AudioFile.CopyToAsync(stream);
            }

            // Save image file
            var imageFileName = $"{Guid.NewGuid()}_{uploadDto.ImageFile.FileName}";
            var imageFilePath = Path.Combine(imagesFolderPath, imageFileName);
            using (var stream = new FileStream(imageFilePath, FileMode.Create))
            {
                await uploadDto.ImageFile.CopyToAsync(stream);
            }

            // Find or create singer
            var singer = await _context.Singers.FirstOrDefaultAsync(s => s.Name.ToLower() == uploadDto.SingerName.ToLower());
            if (singer == null)
            {
                singer = new Singer { Name = uploadDto.SingerName, Bio = "No bio available." };
                _context.Singers.Add(singer);
                await _context.SaveChangesAsync();
            }

            var song = new Song
            {
                Title = uploadDto.Title,
                Genre = uploadDto.Genre,
                FilePath = Path.Combine("uploads", "songs", audioFileName).Replace("\\", "/"),
                ImageUrl = Path.Combine("uploads", "images", imageFileName).Replace("\\", "/"),
                UploadedAt = DateTime.UtcNow,
                SingerId = singer.Id
            };

            _context.Songs.Add(song);
            await _context.SaveChangesAsync();

            var baseUrl = GetBaseUrl();
            return CreatedAtAction(nameof(GetSongs), new { id = song.Id }, new SongDto
            {
                Id = song.Id,
                Title = song.Title,
                SingerName = singer.Name,
                SingerId = singer.Id,
                Genre = song.Genre,
                FileUrl = $"{baseUrl}/api/songs/stream/{song.Id}",
                ImageUrl = $"{baseUrl}/{song.ImageUrl.Replace("\\", "/")}"
            });
        }
    }
}