namespace App.Dtos.Authentication.Response;

public class UserResponseDto
{
    public required Guid UserId { get; init; }
    public required string Username { get; init; } = string.Empty;
    public required string Email { get; init; } = string.Empty;
}