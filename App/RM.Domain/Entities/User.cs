namespace RM.Domain.Entities;

public class User
{
    public Guid UserId { get; init; }
    public string Email { get; init; }
    public string Username { get; init; }
    public string HashedPassword { get; init; }
    
    
    public string Role { get; init; }
    public string TrustedUser { get; init; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpirationTime { get; set; }
}