namespace App.Dtos.Authentication.Request;

public class UserLoginRequestDto
{
    public string? Username { get; init; } = string.Empty;
    public string? Email { get; init; } = string.Empty;
    public required string Password { get; init; } = string.Empty;
}