using System.ComponentModel.DataAnnotations;

namespace App.Domain.Entities;

public class Genre
{
    [Key] public Guid GenreId { get; set; }
    [StringLength(100)] public string Name { get; set; } = string.Empty;
}