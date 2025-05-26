namespace App.Infrastructure.Models.Dtos;

public class TokenResponeDto
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}