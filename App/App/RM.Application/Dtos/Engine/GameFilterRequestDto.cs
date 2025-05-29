using System.ComponentModel.DataAnnotations;

namespace App.RM.Application.Dtos.Engine;

public class GameFilterRequestDto
{
    public string? Genre { get; set; }
    public string? Developer { get; set; }
    [Required, Range(0, 40, ErrorMessage = "Limit must be between 0 and 40")]
    public int? PageSize { get; set; } = 20;
}