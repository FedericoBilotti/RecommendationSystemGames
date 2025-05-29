using System.Text;

namespace App.RM.Infrastructure.Services.Engine;

public class RawgUriBuilder(string baseUri, string apiKey, string middle = "games")
{
    private readonly Dictionary<string, string> _parameters = new();
    
    public RawgUriBuilder WithGenre(string? genre)
    {
        if (!string.IsNullOrWhiteSpace(genre))
            _parameters["genres"] = genre;
        return this;
    }

    public RawgUriBuilder WithDeveloper(string? developer)
    {
        if (!string.IsNullOrWhiteSpace(developer))
            _parameters["developers"] = developer;
        return this;
    }

    public RawgUriBuilder WithPlatforms(IEnumerable<int>? platforms)
    {
        IEnumerable<int> enumerable = platforms as int[] ?? [];
        if (platforms != null && enumerable.Any())
            _parameters["platforms"] = string.Join(",", enumerable);
        return this;
    }

    public RawgUriBuilder WithPageSize(int? pageSize)
    {
        if (pageSize.HasValue)
            _parameters["page_size"] = pageSize.Value.ToString();
        return this;
    }

    public string Build()
    {
        var query = new StringBuilder($"{apiKey}");

        foreach (var param in _parameters)
            query.Append($"&{param.Key}={param.Value}");

        return $"{baseUri}/{middle}{query}";
    }
}