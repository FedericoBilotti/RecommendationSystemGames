using App.Dtos.Games.Requests;
using App.Dtos.Games.Responses;
using RM.Domain.Entities.Games;

namespace App.Interfaces.Engine;

public interface IGameUseCase
{
    Task<GameResponseDto?> CreateAsync(CreateGameRequestDto game, CancellationToken cancellationToken = default);
    Task<Game?> GetByIdAsync(Guid gameId, Guid? userId = default, CancellationToken cancellationToken = default);
    Task<Game?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken cancellationToken = default);
    Task<GameResponseDto?> UpdateAsync(UpdateGameRequestDto game, Guid gameId, Guid? userId = default, CancellationToken cancellationToken = default);
    Task<bool> DeleteByIdAsync(Guid gameId, CancellationToken cancellationToken = default);
    Task<GamesResponseDto> GetAllAsync(GetAllGameRequestDto getAllGameRequestDto, Guid? userId = default, CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(string? title = default, int? yearOfRelease = default, CancellationToken cancellationToken = default);
}