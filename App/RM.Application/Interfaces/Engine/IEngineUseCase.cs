using App.Dtos.Engine;
using App.Dtos.Engine.Developers;
using App.Dtos.Engine.Genre;

namespace App.Interfaces.Engine;

public interface IEngineUseCase
{
    Task<GameGenreFilterResponseDto?> GetGamesByGenreAsync(GameGenreFilterRequestDto gameGenresRequestDto);
    Task<GameDevelopersFilterResponseDto?> GetGamesByDeveloperAsync(GameDevelopersFilterRequestDto requestDto);
    Task<GameFilterResponseDto?> GetGamesByFiltersAsync(GameFilterRequestDto filters);
}