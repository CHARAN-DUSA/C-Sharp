using GameStore.Api.Dtos;

namespace GameStore.Api.Entities;

public static class EntityExtensions  // All extension methods need to be static
{
    public static GameDto AsDto(this Game game)  // The first parameter is the type we want to extend, and it has the 'this' modifier
    {
        return new GameDto(
            game.Id,
            game.Name,
            game.Genre,
            game.Price,
            game.ReleaseDate,
            game.ImageUri
        );
    }
    
    public static CreatedGameDto AsCreatedDto(this Game game)
    {
        return new CreatedGameDto(
            game.Name,
            game.Genre,
            game.Price,
            game.ReleaseDate,
            game.ImageUri
        );
    }
}