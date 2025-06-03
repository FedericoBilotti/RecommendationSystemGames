using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RM.Domain.Entities;

public class Game
{
    [Key] public Guid GameId { get; set; }
    public int UserRating { get; set; } 
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    
    [ForeignKey("GenreId")]
    public ICollection<Genre> Genres { get; set; } = [];
}