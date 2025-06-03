using System.ComponentModel.DataAnnotations;

namespace App.Dtos.Engine.Developers;

public class GameDevelopersFilterRequestDto
{
    [Required, Range(1, 40, ErrorMessage = "Limit must be between 1 and 40")]
    public int limit { get; set; } = 20;
}