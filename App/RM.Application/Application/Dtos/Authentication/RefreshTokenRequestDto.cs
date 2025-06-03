using System.ComponentModel.DataAnnotations;

namespace App.RM.Application.Dtos.Authentication;

public class RefreshTokenRequestDto
{
    [Required] public Guid UserId { get; set; }
    [Required] public required string RefreshToken { get; set; }
}