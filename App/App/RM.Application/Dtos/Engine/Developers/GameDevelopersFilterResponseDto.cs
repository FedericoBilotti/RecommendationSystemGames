using System.Text.Json.Serialization;
using App.RM.Application.Dtos.Engine.Genre;

namespace App.RM.Application.Dtos.Engine.Developers;

public class GameDevelopersFilterResponseDto
{
    [JsonPropertyName("count")] public int Count { get; set; }

    [JsonPropertyName("next")] public string? Next { get; set; }

    [JsonPropertyName("previous")] public string? Previous { get; set; }

    [JsonPropertyName("results")] public List<GameDto> Results { get; set; } = new();
}