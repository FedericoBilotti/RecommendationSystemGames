using App.RM.Application.Dtos.Engine;
using App.RM.Application.Interfaces.Engine;

namespace App.RM.Application.UseCases.Engine;

public class EngineUseCase : IEngineUseCase
{
    private readonly IEngine _engine;
    
    public EngineUseCase(IEngine engine)
    {
        _engine = engine;
    }

    public async Task<GameGenreFilterResponseDto?> GetGamesByGenreAsync(GameGenreFilterRequestDto gameGenresRequestDto)
    {
        return await _engine.GetGamesByGenreAsync(gameGenresRequestDto);
    }
}