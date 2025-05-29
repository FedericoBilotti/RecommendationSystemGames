using System.Text.Json;
using App.RM.Application.Dtos.Engine;
using App.RM.Application.Interfaces;
using App.RM.Application.Interfaces.Engine;
using Microsoft.Extensions.Options;

namespace App.RM.Infrastructure.Services.Engine;

public class EngineService(IDeserializer deserializer, HttpClient http, IOptions<RawgApiSettings> rawgApiSettings) : IEngine
{
    private readonly RawgApiSettings _rawgApiSettings = rawgApiSettings.Value;

    public async Task<GameGenreFilterResponseDto?> GetGamesByGenreAsync(GameGenreFilterRequestDto requestDto)
    {
        string? apiKey = _rawgApiSettings.RawgApikey;

        if (apiKey == null)
        {
            throw new ArgumentNullException(apiKey, "RAWG_APIKEY environment variable is not set");
        }
        
        var uri = $"https://api.rawg.io/api/games?key={apiKey}&genres={requestDto.genre}&page_size={requestDto.limit}";
        return await GetAsync<GameGenreFilterResponseDto>(uri);
    }

    private async Task<T?> GetAsync<T>(string uri)
    {
        HttpResponseMessage res = await http.GetAsync(uri);
        res.EnsureSuccessStatusCode();
        await using Stream stream = await res.Content.ReadAsStreamAsync();
        return await deserializer.DeserializeAsync<T>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
}