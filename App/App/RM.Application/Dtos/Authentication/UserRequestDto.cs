namespace App.RM.Application.Dtos.Authentication;

public class UserRequestDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}