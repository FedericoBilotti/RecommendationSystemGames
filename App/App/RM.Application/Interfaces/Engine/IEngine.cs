using App.RM.Application.Dtos.Engine;
using App.RM.Application.Dtos.Engine.Genre;
using App.RM.Application.Dtos.Engine.Developers;

namespace App.RM.Application.Interfaces.Engine;

public interface IEngine
{
    Task<GameGenreFilterResponseDto?> GetGamesByGenreAsync(GameGenreFilterRequestDto requestDto);
    Task<GameDevelopersFilterResponseDto?> GetGamesByDeveloperAsync(GameDevelopersFilterRequestDto requestDto);
    Task<GameFilterResponseDto?> GetGamesByFiltersAsync(GameFilterRequestDto filters);
}