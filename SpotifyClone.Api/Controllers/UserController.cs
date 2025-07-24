using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpotifyClone.Api.Data;
using SpotifyClone.Api.DTOs;
using SpotifyClone.Api.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SpotifyClone.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpPost("songs/{songId}/like")]
        public async Task<IActionResult> LikeSong(int songId)
        {
            var userId = GetCurrentUserId();

            var songExists = await _context.Songs.AnyAsync(s => s.Id == songId);
            if (!songExists) return NotFound("Song not found.");

            var alreadyLiked = await _context.UserLikedSongs.AnyAsync(uls => uls.UserId == userId && uls.SongId == songId);
            if (alreadyLiked)
            {
                // Already liked, so unlike it
                var likedSong = await _context.UserLikedSongs.FindAsync(userId, songId);
                if (likedSong != null)
                {
                    _context.UserLikedSongs.Remove(likedSong);
                    await _context.SaveChangesAsync();
                    return Ok(new { isLiked = false });
                }
            }
            else
            {
                // Not liked yet, so like it
                var likedSong = new UserLikedSong { UserId = userId, SongId = songId };
                _context.UserLikedSongs.Add(likedSong);
                await _context.SaveChangesAsync();
                return Ok(new { isLiked = true });
            }

            return BadRequest("Could not process like/unlike request.");
        }

        // In Controllers/UserController.cs, add this new method inside the class.

        // In Controllers/UserController.cs

        [HttpGet("songs/liked")]
        public async Task<ActionResult<IEnumerable<SongDto>>> GetLikedSongs()
        {
            var userId = GetCurrentUserId();

            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            // THIS IS THE CORRECTED QUERY
            var likedSongs = await _context.UserLikedSongs
                .Where(uls => uls.UserId == userId)
                .Select(uls => new SongDto // Project directly to the DTO here
                {
                    Id = uls.Song.Id,
                    Title = uls.Song.Title,
                    SingerName = uls.Song.Singer.Name, // This will now work correctly
                    SingerId = uls.Song.SingerId,
                    Genre = uls.Song.Genre,
                    FileUrl = $"{baseUrl}/api/songs/stream/{uls.Song.Id}",
                    ImageUrl = string.IsNullOrEmpty(uls.Song.ImageUrl) ? null : $"{baseUrl}/{uls.Song.ImageUrl}",
                    IsLiked = true
                })
                .ToListAsync();

            return Ok(likedSongs);
        }
    }
}