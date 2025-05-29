using System.ComponentModel.DataAnnotations;

namespace App.RM.Application.Dtos.Engine.Genre;

public class GameGenreFilterRequestDto
{
    // Better to give an enum with genres, for prevent human errors
    [Required] public string genre { get; set; } = "action";
    
    [Required, Range(1, 120, ErrorMessage = "Limit must be between 1 and 120")]
    public int limit { get; set; } = 20;
}