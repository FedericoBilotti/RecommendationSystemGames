using System.Text.Json;
using App.Dtos.Engine;
using App.Dtos.Engine.Developers;
using App.Dtos.Engine.Genre;
using App.Interfaces;
using App.Interfaces.Engine;

namespace App.Services.Engine;

public class EngineService(IDeserializer deserializer, HttpClient http) : IEngine
{
    // , IOptions<RawgApiSettings> rawgApiSettings
    // private readonly RawgApiSettings _rawgApiSettings = rawgApiSettings.Value;

    public async Task<GameGenreFilterResponseDto?> GetGamesByGenreAsync(GameGenreFilterRequestDto requestDto)
    {
        string uri = new RawgUriBuilder(GetRawg(), GetApiKey())
                .WithGenre(requestDto.genre)
                .WithPageSize(requestDto.limit)
                .Build();
        
        return await GetAsync<GameGenreFilterResponseDto>(uri);
    }

    public async Task<GameDevelopersFilterResponseDto?> GetGamesByDeveloperAsync(GameDevelopersFilterRequestDto requestDto)
    {
        string uri = new RawgUriBuilder(GetRawg(), GetApiKey(), "developers")
                .WithPageSize(requestDto.limit)
                .Build();
        
        return await GetAsync<GameDevelopersFilterResponseDto>(uri);
    }
    
    public async Task<GameFilterResponseDto?> GetGamesByFiltersAsync(GameFilterRequestDto filters)
    {
        string uri = new RawgUriBuilder(GetRawg(), GetApiKey())
                .WithGenre(filters.Genre)
                .WithDeveloper(filters.Developer)
                .WithPageSize(filters.PageSize)
                .Build();

        return await GetAsync<GameFilterResponseDto>(uri);
    }

    private async Task<T?> GetAsync<T>(string uri)
    {
        HttpResponseMessage res = await http.GetAsync(uri);
        res.EnsureSuccessStatusCode();
        await using Stream stream = await res.Content.ReadAsStreamAsync();
        return await deserializer.DeserializeAsync<T>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    private string GetApiKey()
    {
        // string? apiKey = _rawgApiSettings.RawgApikey;
        string? apiKey = "";
        return $"?key={apiKey}" ?? throw new ArgumentNullException(apiKey, "RAWG_APIKEY environment variable is not set");
    }

    private static string GetRawg() => "https://api.rawg.io/api";
}