namespace App.Dtos.Games.Requests;

public abstract class PagedRequest
{
    public required int Page { get; init; } = 1;
    public required int PageSize { get; init; } = 25;
}