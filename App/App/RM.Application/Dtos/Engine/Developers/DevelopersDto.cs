using System.Text.Json.Serialization;

namespace App.RM.Application.Dtos.Engine.Developers;

public class DevelopersDto
{
    [JsonPropertyName("id")] public int Id { get; set; }
}   