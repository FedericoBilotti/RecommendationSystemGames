using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace App.RM.Application.Dtos.Engine;

public class GameGenreFilterRequestDto
{
    // Better to give an enum with genres, for prevent human errors
    [FromQuery(Name = "genres"), Required] public string genre { get; set; } = string.Empty;
    
    [FromQuery(Name = "limit"), Required, Range(1, 50, ErrorMessage = "Limit must be between 1 and 50")]
    public int limit { get; set; } = 20;
}