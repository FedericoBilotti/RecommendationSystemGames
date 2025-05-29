using App.RM.Application.Dtos.Engine;

namespace App.RM.Application.Interfaces.Engine;

public interface IEngine
{
    Task<GameGenreFilterResponseDto?> GetGamesByGenreAsync(GameGenreFilterRequestDto requestDto);
}