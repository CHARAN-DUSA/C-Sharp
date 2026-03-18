using GameStore.Api.Data;
using GameStore.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Repositories;

public class EntityFrameworkGamesRepository : IGamesRepository
{
    private readonly GameStoreContext dbContext;

    public EntityFrameworkGamesRepository(GameStoreContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IEnumerable<Game>> GetAllAsync()
    {
        return await dbContext.Games.AsNoTracking().ToListAsync();
    }
    public async Task<Game?> GetAsync(int id)
    {
        return await dbContext.Games.AsNoTracking().FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<Game> CreateAsync(Game newGame)
    {
        dbContext.Games.Add(newGame);
        await dbContext.SaveChangesAsync();
        return newGame;
    }

    public async Task UpdateAsync(int id, Game updatedGame)
    {
        dbContext.Games.Update(updatedGame);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        dbContext.Games.Where(game => game.Id == id).ExecuteDelete();
    }
}
