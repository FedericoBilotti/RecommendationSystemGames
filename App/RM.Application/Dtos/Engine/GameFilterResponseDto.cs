using App.Dtos.Engine.Genre;

namespace App.Dtos.Engine;

public class GameFilterResponseDto
{
    public int Count          { get; set; }
    public string? Next       { get; set; }
    public string? Previous   { get; set; }
    public IEnumerable<GameDto> Results { get; set; } = [];
}