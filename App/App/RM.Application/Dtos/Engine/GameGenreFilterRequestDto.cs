using System.ComponentModel.DataAnnotations;

namespace App.RM.Application.Dtos.Engine;

public class GameGenreFilterRequestDto
{
    // Better to give an enum with genres, for prevent human errors
    [Required] public string genre { get; set; } = "action";
    
    [Required, Range(1, 40, ErrorMessage = "Limit must be between 1 and 50")]
    public int limit { get; set; } = 20;
}