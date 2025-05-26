using System.ComponentModel.DataAnnotations;

namespace App.Domain.Entities.User;

public class User
{
    [Key]
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string HashedPassword { get; set; } = string.Empty;
    // public string Email { get; set; } = string.Empty;
}