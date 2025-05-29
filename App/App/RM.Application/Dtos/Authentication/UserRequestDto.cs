using System.ComponentModel.DataAnnotations;

namespace App.RM.Application.Dtos.Authentication;

public class UserRequestDto
{
    [Required] public string Username { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
    [Required, StringLength(255), EmailAddress] public string Email { get; set; } = string.Empty;
}