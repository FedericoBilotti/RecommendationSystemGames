namespace App.Dtos.Games.Requests;

public class GetAllGameRequestDto : PagedRequest
{
    public string? Title { get; init; }
    public int? YearOfRelease { get; init; }
    public string? SortBy { get; init; } 
}