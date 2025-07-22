using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpotifyClone.Api.Data;
using SpotifyClone.Api.DTOs;
using SpotifyClone.Api.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SpotifyClone.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpGet("song/{songId}")]
        public async Task<ActionResult<CommentDto>> GetCommentsForSong(int songId)
        {
            var comments = await _context.Comments
                .Where(c => c.SongId == songId)
                .Include(c => c.User)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Text = c.Text,
                    Username = c.User.Username,
                    PostedAt = c.PostedAt
                })
                .OrderByDescending(c => c.PostedAt)
                .ToListAsync();

            return Ok(comments);
        }

        [HttpGet("singer/{singerId}")]
        public async Task<ActionResult<CommentDto>> GetCommentsForSinger(int singerId)
        {
            var comments = await _context.Comments
                .Where(c => c.SingerId == singerId)
                .Include(c => c.User)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Text = c.Text,
                    Username = c.User.Username,
                    PostedAt = c.PostedAt
                })
                .OrderByDescending(c => c.PostedAt)
                .ToListAsync();

            return Ok(comments);
        }

        [Authorize]
        [HttpPost("song/{songId}")]
        public async Task<ActionResult<CommentDto>> AddCommentToSong(int songId, CreateCommentDto createDto)
        {
            var songExists = await _context.Songs.AnyAsync(s => s.Id == songId);
            if (!songExists) return NotFound("Song not found.");

            var userId = GetCurrentUserId();
            var comment = new Comment
            {
                Text = createDto.Text,
                PostedAt = DateTime.UtcNow,
                UserId = userId,
                SongId = songId
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(userId);

            return CreatedAtAction(nameof(GetCommentsForSong), new { songId = songId }, new CommentDto
            {
                Id = comment.Id,
                Text = comment.Text,
                Username = user.Username,
                PostedAt = comment.PostedAt
            });
        }

        [Authorize]
        [HttpPost("singer/{singerId}")]
        public async Task<ActionResult<CommentDto>> AddCommentToSinger(int singerId, CreateCommentDto createDto)
        {
            var singerExists = await _context.Singers.AnyAsync(s => s.Id == singerId);
            if (!singerExists) return NotFound("Singer not found.");

            var userId = GetCurrentUserId();
            var comment = new Comment
            {
                Text = createDto.Text,
                PostedAt = DateTime.UtcNow,
                UserId = userId,
                SingerId = singerId
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(userId);

            return CreatedAtAction(nameof(GetCommentsForSinger), new { singerId = singerId }, new CommentDto
            {
                Id = comment.Id,
                Text = comment.Text,
                Username = user.Username,
                PostedAt = comment.PostedAt
            });
        }
    }
}