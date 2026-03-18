using GameStore.Api.Dtos;
using GameStore.Api.Entities;

namespace GameStore.Api.Repositories;

public class InMemGamesRepository : IGamesRepository
{

    private readonly List<Game> games = new List<Game>
{
    new Game
    {
        Id          = 1,
        Name        = "The Legend of Zelda: Breath of the Wild",
        Genre       = "Action-Adventure",
        Price       = 59.99m,
        ReleaseDate = new DateTime(2017, 3, 3),
        ImageUri    = "https://example.com/zelda.jpg"
    },
    new Game
    {
        Id          = 2,
        Name        = "God of War",
        Genre       = "Action",
        Price       = 49.99m,
        ReleaseDate = new DateTime(2018, 4, 20),
        ImageUri    = "https://example.com/godofwar.jpg"
    },
    new Game
    {
        Id          = 3,
        Name        = "Red Dead Redemption 2",
        Genre       = "Action-Adventure",
        Price       = 59.99m,
        ReleaseDate = new DateTime(2018, 10, 26),
        ImageUri    = "https://example.com/rdr2.jpg"
    }
};

    public async Task<IEnumerable<Game>> GetAllAsync()
    {
        return await Task.FromResult(games);
    }

    public async Task<Game?> GetAsync(int id)
    {
        return await Task.FromResult(games.FirstOrDefault(g => g.Id == id));
    }

    public async Task<Game> CreateAsync(Game newGame)
    {
        newGame.Id = games.Max(g => g.Id) + 1;
        games.Add(newGame);
        return newGame;
    }

    public async Task UpdateAsync(int id, Game updatedGame)
    {
        var index = games.FindIndex(g => g.Id == id);
        games[index] = updatedGame;
    }

    public async Task DeleteAsync(int id)
    {
        var index = games.FindIndex(g => g.Id == id);
        games.RemoveAt(index);
        await Task.CompletedTask;
    }


}