namespace App.Dtos.Games.Requests;

public class GetAllGameRequest
{
    public required string? Title { get; init; }
    public required int? YearOfRelease { get; init; }
}