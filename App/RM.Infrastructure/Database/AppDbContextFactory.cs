using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DotNetEnv;

namespace RM.Infrastructure.Database;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        string envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "RM.Presentation", ".env");
        Env.Load(envPath);

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        string connectionString = Environment.GetEnvironmentVariable("USER_DATABASE")!;

        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}