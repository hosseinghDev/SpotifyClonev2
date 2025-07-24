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
        private readonly ILogger<SongsController> _logger; // <-- ADD LOGGER

        public SongsController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment, ILogger<SongsController> logger) // <-- INJECT LOGGER
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger; // <-- INITIALIZE LOGGER
        }

        private string GetBaseUrl()
        {
            // Use the current request's scheme and host for dynamic URL generation
            var request = HttpContext.Request;
            return $"{request.Scheme}://{request.Host}";
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SongDto>>> GetSongs([FromQuery] string? search)
        {
            // Start with a clean query
            var query = _context.Songs.Include(s => s.Singer).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                // Apply search filter if provided
                var searchTerm = search.ToLower();
                query = query.Where(s => s.Title.ToLower().Contains(searchTerm) ||
                                         s.Singer.Name.ToLower().Contains(searchTerm) ||
                                         s.Genre.ToLower().Contains(searchTerm));
            }

            var baseUrl = GetBaseUrl();

            // Project the results into DTOs
            var songs = await query
                .Select(s => new SongDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    SingerName = s.Singer.Name,
                    SingerId = s.SingerId,
                    Genre = s.Genre,
                    // The streaming URL points to our controller action
                    FileUrl = $"{baseUrl}/api/songs/stream/{s.Id}",
                    // The image URL points directly to the static file path
                    ImageUrl = string.IsNullOrEmpty(s.ImageUrl) ? null : $"{baseUrl}/{s.ImageUrl}"
                })
                .ToListAsync();

            return Ok(songs); // <-- ALWAYS WRAP IN OK() FOR CONSISTENCY
        }

        [HttpGet("stream/{id}")]
        public async Task<IActionResult> StreamSong(int id)
        {
            var song = await _context.Songs.FindAsync(id);
            if (song == null || string.IsNullOrEmpty(song.FilePath))
            {
                _logger.LogWarning($"StreamSong: Song with ID {id} or its FilePath not found in database.");
                return NotFound("Song metadata not found.");
            }

            // --- New, More Robust Path Logic ---
            // Get the base wwwroot path
            var wwwRootPath = _hostingEnvironment.WebRootPath;

            // Sanitize the relative path from the database to remove any leading slashes
            // that might confuse the path logic.
            var relativePath = song.FilePath.TrimStart('/', '\\');

            // Manually combine the path components. This is more explicit than Path.Combine
            // which can sometimes have subtle cross-platform differences.
            var fullPath = Path.Combine(wwwRootPath, relativePath);

            _logger.LogInformation($"StreamSong: Attempting to stream file from fully constructed path: {fullPath}");

            if (!System.IO.File.Exists(fullPath))
            {
                _logger.LogError($"StreamSong: CRITICAL - File does not exist on server at path: {fullPath}");
                // For debugging, let's also log what's in the directory.
                try
                {
                    var directoryPath = Path.GetDirectoryName(fullPath);
                    if (Directory.Exists(directoryPath))
                    {
                        var filesInDir = Directory.GetFiles(directoryPath);
                        _logger.LogInformation($"Files in directory '{directoryPath}': {string.Join(", ", filesInDir)}");
                    }
                    else
                    {
                        _logger.LogWarning($"Directory does not exist: {directoryPath}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while trying to list directory contents for debugging.");
                }

                return NotFound("Audio file not found on the server.");
            }

            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
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