using Microsoft.AspNetCore.Mvc;

namespace App.RM.Application.Dtos.Engine;

public class GenreFilterDto
{
    [FromQuery(Name = "genreId")] public int? GenreId { get; set; }
    [FromQuery(Name = "name")] public string? Name { get; set; }
    [FromQuery(Name = "limit")] public int? Limit { get; set; }
}