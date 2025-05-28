using System.Text.Json;
using App.RM.Application.Dtos.Engine;
using App.RM.Application.Interfaces;
using App.RM.Application.Interfaces.Engine;

namespace App.RM.Infrastructure.Services.Engine;

public class EngineService(IDeserializer deserializer, HttpClient http, string apiKey) : IEngine
{
    public async Task<GameGenreFilterResponseDto?> GetGamesByGenreAsync(GameGenreFilterRequestDto gameGenreFilterRequestDto)
    {
        var uri = $"games?key={apiKey}&genres={gameGenreFilterRequestDto.genre}&page={gameGenreFilterRequestDto.limit}";
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