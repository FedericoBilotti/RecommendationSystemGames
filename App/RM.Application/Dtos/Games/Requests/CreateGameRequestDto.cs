namespace App.Dtos.Games.Requests;

public class CreateGameRequestDto
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required int YearOfRelease { get; init; }
    public required IEnumerable<string> Genre { get; init; } = Enumerable.Empty<string>();
}