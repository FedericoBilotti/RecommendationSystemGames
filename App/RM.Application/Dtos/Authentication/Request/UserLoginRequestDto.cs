namespace App.Dtos.Authentication.Request;

public class UserLoginRequestDto
{
    public string? Username { get; init; }
    public string? Email { get; init; }
    public required string Password { get; init; }
}