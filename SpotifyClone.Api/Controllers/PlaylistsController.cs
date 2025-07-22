using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpotifyClone.Api.Data;
using SpotifyClone.Api.DTOs;
using SpotifyClone.Api.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SpotifyClone.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PlaylistsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PlaylistsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        private string GetBaseUrl()
        {
            return $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
        }

        [HttpGet]
        public async Task<ActionResult<PlaylistDto>> GetUserPlaylists()
        {
            var userId = GetCurrentUserId();
            var playlists = await _context.Playlists
                .Where(p => p.UserId == userId)
                .Select(p => new PlaylistDto
                {
                    Id = p.Id,
                    Name = p.Name
                })
                .ToListAsync();

            return Ok(playlists);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlaylistDto>> GetPlaylist(int id)
        {
            var userId = GetCurrentUserId();
            var baseUrl = GetBaseUrl();
            var playlist = await _context.Playlists
                .Include(p => p.User)
                .Include(p => p.PlaylistSongs)
                .ThenInclude(ps => ps.Song)
                .ThenInclude(s => s.Singer)
                .Where(p => p.Id == id && p.UserId == userId)
                .Select(p => new PlaylistDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    OwnerUsername = p.User.Username,
                    Songs = p.PlaylistSongs.Select(ps => new SongDto
                    {
                        Id = ps.Song.Id,
                        Title = ps.Song.Title,
                        SingerName = ps.Song.Singer.Name,
                        SingerId = ps.Song.SingerId,
                        Genre = ps.Song.Genre,
                        FileUrl = $"{baseUrl}/api/songs/stream/{ps.Song.Id}",
                        ImageUrl = string.IsNullOrEmpty(ps.Song.ImageUrl) ? null : $"{baseUrl}/{ps.Song.ImageUrl.Replace("\\", "/")}"
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (playlist == null)
            {
                return NotFound();
            }

            return Ok(playlist);
        }

        [HttpPost]
        public async Task<ActionResult<PlaylistDto>> CreatePlaylist(CreatePlaylistDto createDto)
        {
            var userId = GetCurrentUserId();
            var playlist = new Playlist
            {
                Name = createDto.Name,
                UserId = userId
            };

            _context.Playlists.Add(playlist);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlaylist), new { id = playlist.Id }, new PlaylistDto { Id = playlist.Id, Name = playlist.Name });
        }

        [HttpPost("{playlistId}/songs")]
        public async Task<IActionResult> AddSongToPlaylist(int playlistId, [FromBody] AddSongToPlaylistDto addDto)
        {
            var userId = GetCurrentUserId();
            var playlist = await _context.Playlists.FirstOrDefaultAsync(p => p.Id == playlistId && p.UserId == userId);
            if (playlist == null) return NotFound("Playlist not found or you don't have permission.");

            var songExists = await _context.Songs.AnyAsync(s => s.Id == addDto.SongId);
            if (!songExists) return NotFound("Song not found.");

            var alreadyInPlaylist = await _context.PlaylistSongs.AnyAsync(ps => ps.PlaylistId == playlistId && ps.SongId == addDto.SongId);
            if (alreadyInPlaylist) return BadRequest("Song is already in the playlist.");

            var playlistSong = new PlaylistSong { PlaylistId = playlistId, SongId = addDto.SongId };
            _context.PlaylistSongs.Add(playlistSong);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{playlistId}/songs/{songId}")]
        public async Task<IActionResult> RemoveSongFromPlaylist(int playlistId, int songId)
        {
            var userId = GetCurrentUserId();
            var playlist = await _context.Playlists.FirstOrDefaultAsync(p => p.Id == playlistId && p.UserId == userId);
            if (playlist == null) return NotFound("Playlist not found or you don't have permission.");

            var playlistSong = await _context.PlaylistSongs.FirstOrDefaultAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId);
            if (playlistSong == null) return NotFound("Song not found in playlist.");

            _context.PlaylistSongs.Remove(playlistSong);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}