namespace App.Dtos.Authentication.Response;

public class TokenResponseDto
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}