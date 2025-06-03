using RM.Domain.Entities.Games;

namespace RM.Domain.Entities;

public class User
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string HashedPassword { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    
    public IEnumerable<Game> FavoriteGames { get; init; } = [];
    
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpirationTime { get; set; }
}