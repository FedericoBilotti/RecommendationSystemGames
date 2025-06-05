using Microsoft.EntityFrameworkCore;
using RM.Domain.Entities;
using RM.Domain.Entities.Games;

namespace RM.Infrastructure.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Game> Games { get; set; }
    // public DbSet<Genre> Genres { get; set; }
    //
    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     modelBuilder.Entity<User>().ToTable("Users");
    //
    //     modelBuilder.Entity<Game>(entity =>
    //     {
    //         entity.ToTable("Games");
    //         entity.HasKey(g => g.GameId);
    //         entity.Property(g => g.Title).IsRequired();
    //         entity.Property(g => g.Description).IsRequired();
    //         entity.Property(g => g.YearOfRelease).IsRequired();
    //         entity.Property(g => g.UserRating);
    //     });
    //
    //     modelBuilder.Entity<Genre>(entity =>
    //     {
    //         entity.ToTable("Genres");
    //         entity.HasKey(g => g.GenreId);
    //         entity.Property(g => g.Name).IsRequired();
    //     });
    //
    //     modelBuilder.Entity<Game>()
    //             .HasMany(g => g.Genres)
    //             .WithMany(g => g.Games)
    //             .UsingEntity<Dictionary<string, object>>("GameGenres",
    //             entityTypeBuilder => entityTypeBuilder.HasOne<Genre>().WithMany().HasForeignKey("GenreId").HasConstraintName("FK_GameGenres_Genres").OnDelete(DeleteBehavior.Cascade),
    //             entityTypeBuilder => entityTypeBuilder.HasOne<Game>().WithMany().HasForeignKey("GameId").HasConstraintName("FK_GameGenres_Games").OnDelete(DeleteBehavior.Cascade), 
    //             entityTypeBuilder => {
    //                 entityTypeBuilder.HasKey("GameId", "GenreId");
    //                 entityTypeBuilder.ToTable("GameGenres");
    //             });
    // }
}