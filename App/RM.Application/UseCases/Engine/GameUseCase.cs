using App.Dtos.Games.Requests;
using App.Dtos.Games.Responses;
using App.Interfaces.Engine;
using App.Mappers;
using App.Services.Validators.Games;
using FluentValidation;
using RM.Domain.Entities;
using RM.Domain.Entities.Games;

namespace App.UseCases.Engine;

public class GameUseCase : IGameUseCase
{
    private readonly IGamesRepository _gamesRepository;
    private readonly IRatingRepository _ratingRepository;
    private readonly IValidator<Game> _gameValidatorService;
    private readonly IValidator<GetAllGameRequestDto> _getAllDtoValidator;
    private readonly IValidator<GetAllGameOptions> _optionsValidator;

    public GameUseCase(IGamesRepository gamesRepository, IRatingRepository ratingRepository, IValidator<Game> gameValidatorService, IValidator<GetAllGameRequestDto> getAllDtoValidator, 
            IValidator<GetAllGameOptions> optionsValidator)
    {
        _gamesRepository = gamesRepository;
        _ratingRepository = ratingRepository;
        _gameValidatorService = gameValidatorService;
        _getAllDtoValidator = getAllDtoValidator;
        _optionsValidator = optionsValidator;
    }

    public async Task<GameResponseDto?> CreateAsync(CreateGameRequestDto createGameRequestDto, CancellationToken cancellationToken = default)
    {
        Game game = createGameRequestDto.MapToGame();
        await _gameValidatorService.ValidateAndThrowAsync(game, cancellationToken);
        
        bool gameWasCreated = await _gamesRepository.CreateAsync(game, cancellationToken);

        return gameWasCreated ? game.MapToResponse() : null;
    }

    public Task<Game?> GetByIdAsync(Guid gameId, Guid? userId = default, CancellationToken cancellationToken = default)
    {
        return _gamesRepository.GetByIdAsync(gameId, userId, cancellationToken);
    }

    public Task<Game?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken cancellationToken = default)
    {
        return _gamesRepository.GetBySlugAsync(slug, userId, cancellationToken);
    }

    public async Task<GameResponseDto?> UpdateAsync(UpdateGameRequestDto gameDto, Guid gameId, Guid? userId = default, CancellationToken cancellationToken = default)
    {
        Game game = gameDto.MapToGame(gameId);
        await _gameValidatorService.ValidateAndThrowAsync(game, cancellationToken);
        
        bool movieExists = await _gamesRepository.ExistsByIdAsync(game.GameId, cancellationToken);

        if (!movieExists)
        {
            return null;
        }
        
        await _gamesRepository.UpdateAsync(game, cancellationToken);

        if (!userId.HasValue)
        {
            float? rating = await _ratingRepository.GetRatingAsync(game.GameId, cancellationToken);
            game.Rating = rating;
            return game.MapToResponse();
        }

        (float? Rating, int? User) ratings = await _ratingRepository.GetUserRatingAsync(game.GameId, userId.Value, cancellationToken);
        
        game.Rating = ratings.Rating;
        game.UserRating = ratings.User;
        
        return game.MapToResponse();
    }

    public Task<bool> DeleteByIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        return _gamesRepository.DeleteByIdAsync(gameId, cancellationToken);
    }

    public async Task<GamesResponseDto> GetAllAsync(GetAllGameRequestDto getAllGameRequestDto, Guid? userId = default, CancellationToken cancellationToken = default)
    {
        await _getAllDtoValidator.ValidateAndThrowAsync(getAllGameRequestDto, cancellationToken);
        
        GetAllGameOptions gameOptions = getAllGameRequestDto.MapToOptions().WithId(userId ?? Guid.Empty);
        
        await _optionsValidator.ValidateAndThrowAsync(gameOptions, cancellationToken);
        
        IEnumerable<Game> games = await _gamesRepository.GetAllAsync(gameOptions, cancellationToken);
        
        int count = await GetCountAsync(getAllGameRequestDto.Title, getAllGameRequestDto.YearOfRelease, cancellationToken);

        GamesResponseDto gameResponse = games.MapToResponse(getAllGameRequestDto.Page, getAllGameRequestDto.PageSize, count);
        
        return gameResponse;
    }

    public Task<int> GetCountAsync(string? title = default, int? yearOfRelease = default, CancellationToken cancellationToken = default)
    {
        return _gamesRepository.GetCountAsync(title, yearOfRelease, cancellationToken);
    }
}