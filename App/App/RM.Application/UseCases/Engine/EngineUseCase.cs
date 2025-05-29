using App.RM.Application.Dtos.Engine;
using App.RM.Application.Interfaces.Engine;

namespace App.RM.Application.UseCases.Engine;

public class EngineUseCase(IEngine engine) : IEngineUseCase
{
    public async Task<GameGenreFilterResponseDto?> GetGamesByGenreAsync(GameGenreFilterRequestDto gameGenresRequestDto)
    {
        return await engine.GetGamesByGenreAsync(gameGenresRequestDto);
    }
}