using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpotifyClone.Api.Data;
using SpotifyClone.Api.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyClone.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SingersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SingersController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string GetBaseUrl()
        {
            return $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SingerDto>> GetSinger(int id)
        {
            var baseUrl = GetBaseUrl();
            var singer = await _context.Singers
                .Include(s => s.Songs).ThenInclude(song => song.Singer)
                .Include(s => s.Comments).ThenInclude(c => c.User)
                .Where(s => s.Id == id)
                .Select(s => new SingerDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Bio = s.Bio,
                    Songs = s.Songs.Select(song => new SongDto
                    {
                        Id = song.Id,
                        Title = song.Title,
                        SingerName = s.Name,
                        SingerId = s.Id,
                        Genre = song.Genre,
                        FileUrl = $"{baseUrl}/api/songs/stream/{song.Id}",
                        ImageUrl = string.IsNullOrEmpty(song.ImageUrl) ? null : $"{baseUrl}/{song.ImageUrl.Replace("\\", "/")}"
                    }).ToList(),
                    Comments = s.Comments.Select(c => new CommentDto
                    {
                        Id = c.Id,
                        Text = c.Text,
                        Username = c.User.Username,
                        PostedAt = c.PostedAt
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (singer == null)
            {
                return NotFound();
            }

            return Ok(singer);
        }
    }
}