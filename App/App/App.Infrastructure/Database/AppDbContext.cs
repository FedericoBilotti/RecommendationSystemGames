using App.App.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
}