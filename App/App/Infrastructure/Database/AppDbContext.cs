using App.Application.Interfaces;
using App.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IDbContext
{
    public DbSet<User> Users { get; set; }
}