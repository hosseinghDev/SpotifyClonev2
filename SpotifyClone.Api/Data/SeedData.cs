using Microsoft.EntityFrameworkCore;
using SpotifyClone.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyClone.Api.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                if (context.Users.Any())
                {
                    return; // DB has been seeded
                }

                // Create Password Hash/Salt helper
                void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
                {
                    using (var hmac = new HMACSHA512())
                    {
                        passwordSalt = hmac.Key;
                        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                    }
                }

                // Users
                CreatePasswordHash("password123", out byte[] hash1, out byte[] salt1);
                var user1 = new User { Username = "alice", PasswordHash = hash1, PasswordSalt = salt1 };

                CreatePasswordHash("password123", out byte[] hash2, out byte[] salt2);
                var user2 = new User { Username = "bob", PasswordHash = hash2, PasswordSalt = salt2 };

                context.Users.AddRange(user1, user2);
                await context.SaveChangesAsync();

                // Singers
                var singer1 = new Singer { Name = "The Midnight", Bio = "Synthwave duo from Los Angeles." };
                var singer2 = new Singer { Name = "FM-84", Bio = "Dreamy synthpop from San Francisco." };

                context.Singers.AddRange(singer1, singer2);
                await context.SaveChangesAsync();

                // Songs
                // Created folders and files manually for the seed to work
                // API_PROJECT_ROOT/wwwroot/uploads/songs/
                // API_PROJECT_ROOT/wwwroot/uploads/images/
                var song1 = new Song { Title = "Sunset", SingerId = singer1.Id, Genre = "Synthwave", FilePath = "uploads/songs/the_midnight_sunset.mp3", ImageUrl = "uploads/images/the_midnight_cover.jpg", UploadedAt = DateTime.UtcNow };
                var song2 = new Song { Title = "Vampires", SingerId = singer1.Id, Genre = "Synthwave", FilePath = "uploads/songs/the_midnight_vampires.mp3", ImageUrl = "uploads/images/the_midnight_cover.jpg", UploadedAt = DateTime.UtcNow };
                var song3 = new Song { Title = "Running In The Night", SingerId = singer2.Id, Genre = "Synthpop", FilePath = "uploads/songs/fm84_running.mp3", ImageUrl = "uploads/images/fm84_cover.jpg", UploadedAt = DateTime.UtcNow };

                context.Songs.AddRange(song1, song2, song3);
                await context.SaveChangesAsync();

                // Playlists
                var playlist1 = new Playlist { Name = "Late Night Drives", UserId = user1.Id };
                var playlist2 = new Playlist { Name = "Focus Mix", UserId = user1.Id };
                context.Playlists.AddRange(playlist1, playlist2);
                await context.SaveChangesAsync();

                // Playlist Songs
                var ps1 = new PlaylistSong { PlaylistId = playlist1.Id, SongId = song1.Id };
                var ps2 = new PlaylistSong { PlaylistId = playlist1.Id, SongId = song3.Id };
                var ps3 = new PlaylistSong { PlaylistId = playlist2.Id, SongId = song2.Id };
                context.PlaylistSongs.AddRange(ps1, ps2, ps3);
                await context.SaveChangesAsync();

                // Comments
                var comment1 = new Comment { Text = "Absolute classic!", UserId = user1.Id, SongId = song1.Id, PostedAt = DateTime.UtcNow };
                var comment2 = new Comment { Text = "Love this band.", UserId = user2.Id, SingerId = singer1.Id, PostedAt = DateTime.UtcNow };
                context.Comments.AddRange(comment1, comment2);
                await context.SaveChangesAsync();
            }
        }
    }
}