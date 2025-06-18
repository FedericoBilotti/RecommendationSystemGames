namespace RM.Domain.Entities;

public class User
{
    public required Guid UserId { get; init; }
    public required string Email { get; init; }
    public required string Username { get; init; }
    public required string HashedPassword { get; init; }
    public required string Role { get; init; }
    
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpirationTimeUtc { get; set; }
}