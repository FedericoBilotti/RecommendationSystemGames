using App.RM.Application.Dtos.Engine;
using App.RM.Application.Dtos.Engine.Developers;
using App.RM.Application.Dtos.Engine.Genre;
using App.RM.Application.Interfaces.Engine;

namespace App.RM.Application.UseCases.Engine;

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