namespace App.Dtos.Authentication.Request;

public class Token
{
    public required Guid UserId { get; init; }
    public required string RefreshToken { get; init; }
}