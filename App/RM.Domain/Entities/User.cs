using RM.Domain.Entities.Games;

namespace RM.Domain.Entities;

public class User
{
    public Guid UserId { get; init; }
    public required string Email { get; init; }
    public required string Username { get; init; }
    public required string HashedPassword { get; init; }
    public required string Role { get; init; }
    
    public IEnumerable<Game> FavoriteGames { get; init; } = [];
    
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpirationTime { get; set; }
}