namespace App.Dtos.Games.Responses;

public class PagedResponse<TResponse>
{
    public required IEnumerable<TResponse> GamesResponseDtos { get; init; } = Enumerable.Empty<TResponse>();
    
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public bool HasNextPage => TotalCount > Page * PageSize;
}