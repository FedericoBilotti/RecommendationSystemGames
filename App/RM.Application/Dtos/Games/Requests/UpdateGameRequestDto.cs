namespace App.Dtos.Games.Requests;

public class UpdateGameRequestDto
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required int YearOfRelease { get; init; }
    public required IEnumerable<string> Genre { get; init; } = [];
}