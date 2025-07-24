using Microsoft.EntityFrameworkCore;
using SpotifyClone.Api.Models;

namespace SpotifyClone.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Singer> Singers { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistSong> PlaylistSongs { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<UserLikedSong> UserLikedSongs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite key for PlaylistSong
            modelBuilder.Entity<PlaylistSong>()
                .HasKey(ps => new { ps.PlaylistId, ps.SongId });

            // Relationships for PlaylistSong
            modelBuilder.Entity<PlaylistSong>()
                .HasOne(ps => ps.Playlist)
                .WithMany(p => p.PlaylistSongs)
                .HasForeignKey(ps => ps.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlaylistSong>()
                .HasOne(ps => ps.Song)
                .WithMany(s => s.PlaylistSongs)
                .HasForeignKey(ps => ps.SongId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationships for Comment
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Song)
                .WithMany(s => s.Comments)
                .HasForeignKey(c => c.SongId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Singer)
                .WithMany(s => s.Comments)
                .HasForeignKey(c => c.SingerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Composite key for UserLikedSong
            modelBuilder.Entity<UserLikedSong>()
                .HasKey(uls => new { uls.UserId, uls.SongId });

            // Relationships for UserLikedSong
            modelBuilder.Entity<UserLikedSong>()
                .HasOne(uls => uls.User)
                .WithMany(u => u.LikedSongs)
                .HasForeignKey(uls => uls.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserLikedSong>()
                .HasOne(uls => uls.Song)
                .WithMany(s => s.LikedByUsers)
                .HasForeignKey(uls => uls.SongId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}