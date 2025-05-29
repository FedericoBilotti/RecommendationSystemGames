using System.Text.Json.Serialization;

namespace App.RM.Application.Dtos.Engine;

public class GameGenreFilterResponseDto
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("next")]
    public string? Next { get; set; }

    [JsonPropertyName("previous")]
    public string? Previous { get; set; }

    [JsonPropertyName("results")]
    public List<GameDto> Results { get; set; } = new();
}
