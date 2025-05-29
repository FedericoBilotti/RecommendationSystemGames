using Microsoft.AspNetCore.Mvc;

namespace App.RM.Application.Dtos.Engine;

public class GameGenreFilterRequestDto
{
    [FromQuery(Name = "genres")] public string genre { get; set; } = string.Empty;
    [FromQuery(Name = "limit")] public int limit { get; set; } = 1;
}