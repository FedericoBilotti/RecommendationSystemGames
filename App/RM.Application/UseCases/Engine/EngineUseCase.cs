using App.Dtos.Engine;
using App.Dtos.Engine.Developers;
using App.Dtos.Engine.Genre;
using App.Interfaces.Engine;

namespace App.UseCases.Engine;

public class EngineUseCase(IEngine engine) : IEngineUseCase
{
    public async Task<GameGenreFilterResponseDto?> GetGamesByGenreAsync(GameGenreFilterRequestDto gameGenresRequestDto)
    {
        return await engine.GetGamesByGenreAsync(gameGenresRequestDto);
    }

    public async Task<GameDevelopersFilterResponseDto?> GetGamesByDeveloperAsync(GameDevelopersFilterRequestDto requestDto)
    {
        return await engine.GetGamesByDeveloperAsync(requestDto);
    }

    public async Task<GameFilterResponseDto?> GetGamesByFiltersAsync(GameFilterRequestDto filters)
    {
        return await engine.GetGamesByFiltersAsync(filters);
    }
}