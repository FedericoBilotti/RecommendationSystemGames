using App.RM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.RM.Infrastructure.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Genre> Genres { get; set; }
}