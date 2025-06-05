using App.Interfaces.Engine;
using Microsoft.EntityFrameworkCore;
using RM.Domain.Entities.Games;
using RM.Infrastructure.Database;

namespace RM.Infrastructure.Data;

public class GamesRepository(AppDbContext appDbContext) : IGamesRepository
{
    public async Task<bool> CreateAsync(Game game, CancellationToken cancellationToken = default)
    {        
        foreach (Genre genreName in game.Genres.ToList())
        {
            Genre? existingGenre = await appDbContext.Genres.FirstOrDefaultAsync(g => g.Name == genreName.Name, cancellationToken);

            if (existingGenre != null)
            {
                game.Genres.Add(existingGenre);
                continue;
            }

            var newGenre = new Genre
            {
                GenreId = Guid.NewGuid(),
                Name = genreName.Name
            };
            game.Genres.Add(newGenre);
        }
        
        await appDbContext.Games.AddAsync(game, cancellationToken);
        int result = await appDbContext.SaveChangesAsync(cancellationToken);
        return result > 0;
    }

    public Task<Game?> GetByIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Game?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(Game game, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteByIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Game>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsByIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}