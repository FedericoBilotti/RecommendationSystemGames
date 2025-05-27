using System.ComponentModel.DataAnnotations;

namespace App.Domain.Entities;

public class User
{
    [Key] public Guid UserId { get; set; }
    [StringLength(255)] public string Email { get; set; } = string.Empty;
    [StringLength(100)] public string Username { get; set; } = string.Empty;
    [StringLength(100)] public string HashedPassword { get; set; } = string.Empty;
    [StringLength(15)] public string Role { get; set; } = string.Empty;
    
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpirationTime { get; set; }
}