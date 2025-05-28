using App.RM.Application.Dtos.Engine;

namespace App.RM.Application.Interfaces.Engine;

public interface IEngineUseCase
{
    Task<GameGenreFilterResponseDto?> GetGamesByGenreAsync(GameGenreFilterRequestDto gameGenresRequestDto);
}