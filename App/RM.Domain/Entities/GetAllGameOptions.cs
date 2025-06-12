namespace RM.Domain.Entities;

public class GetAllGameOptions
{
    public string? Title { get; set; }
    public int? YearOfRelease { get; set; }
    public Guid? UserId { get; set; }
}