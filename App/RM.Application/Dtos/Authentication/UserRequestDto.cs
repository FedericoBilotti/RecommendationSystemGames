using System.ComponentModel.DataAnnotations;

namespace App.Dtos.Authentication;

public class UserRequestDto
{
    [Required, StringLength(30)] public string Username { get; set; } = string.Empty;
    [Required, StringLength(50)] public string Password { get; set; } = string.Empty;
    [Required, StringLength(255), EmailAddress] public string Email { get; set; } = string.Empty;
}