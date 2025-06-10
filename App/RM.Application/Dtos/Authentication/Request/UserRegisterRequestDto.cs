namespace App.Dtos.Authentication.Request;

public class UserRegisterRequestDto
{
    public required string Username { get; init; } = string.Empty;
    public required string Email { get; init; } = string.Empty;
    public required string Password { get; init; } = string.Empty;
}