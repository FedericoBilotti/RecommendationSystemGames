using System.ComponentModel.DataAnnotations;

namespace App.Dtos.Authentication;

public class RefreshTokenRequestDto
{
    [Required] public Guid UserId { get; set; }
    [Required] public required string RefreshToken { get; set; }
}