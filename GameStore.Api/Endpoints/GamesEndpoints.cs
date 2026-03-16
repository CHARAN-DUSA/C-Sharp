using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GameEndpoints
{

    const string GetGameEndpointName = "Charan";

    // private static readonly List<GameSummaryDto> games = [
    //     new (1, "Street Fighter II","Fighting",1,55.99M,new DateOnly(2005,3,26)),
    //         new (2, "Pubg","ESports",2,99.99M,new DateOnly(2006,7,7)),
    //             new (3, "AM41","Love",3,26.41M,new DateOnly(2022,9,11)),
    // ];

    public static void MapGameEndpoints(this WebApplication app)
    {

        var group = app.MapGroup("/games").WithTags("Charan"); // You can use any name as withTags, it's just for grouping endpoints in Swagger UI. It doesn't affect the URL or functionality of the endpoints.And it was created to doesn't need to write the /games in very request

        // =============================
        // GET /games
        // =============================
        group.MapGet("/", async (GameStoreContext dbContext) =>
            await dbContext.Games
                .Select(game => new GameSummaryDto(
                    game.Id,
                    game.Name,
                    game.Genre!.Name,
                    game.GenreId,
                    game.Price,
                    game.ReleaseDate))
                    .AsNoTracking()
                    .ToListAsync());

        // =============================
        // Get /games/{id}
        // =============================
        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            var game = await dbContext.Games.FindAsync(id);
            return game is null ? Results.NotFound() : Results.Ok(
                new GameDetailsDto(
                    game.Id,
                    game.Name,
                    game.GenreId,
                    game.Price,
                    game.ReleaseDate));
        })
        .WithName(GetGameEndpointName);  // Withname is used do not need to use the url again by replacing with a string



        // =============================
        // POST /games
        // =============================
        group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            if (string.IsNullOrEmpty(newGame.Name) || newGame.GenreId <= 0 || newGame.Price <= 0)
            {
                return Results.BadRequest();
            }
            Game game = new()
            {
                Name = newGame.Name,
                GenreId = newGame.GenreId,
                Price = newGame.Price,
                ReleaseDate = newGame.ReleaseDate
            };
            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();
            GameDetailsDto gameDto = new(
                game.Id,
                game.Name,
                game.GenreId,
                game.Price,
                game.ReleaseDate
            ); // Writing these lines refer the output
            return Results.CreatedAtRoute(GetGameEndpointName, new { id = gameDto.Id }, gameDto);
        });



        // =============================
        // PUT/ games/1
        // =============================
        group.MapPut("/{id}", async (int id, UpdateGameDto updateGame, GameStoreContext dbContext) =>
        {
            // // var index = games.FindIndex(game => game.Id == id);
            // var existingGame = await dbContext.Games.FindAsync(id);

            // if (existingGame is null)
            // {
            //     return Results.NotFound();
            // }

            // existingGame.Name = updateGame.Name;
            // existingGame.GenreId = updateGame.GenreId;
            // existingGame.Price = updateGame.Price;
            // existingGame.ReleaseDate = updateGame.ReleaseDate;

            // await dbContext.SaveChangesAsync();
            // return Results.NoContent();

            var rowsAffected = await dbContext.Games.Where(game => game.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(game => game.Name, updateGame.Name)
                    .SetProperty(game => game.GenreId, updateGame.GenreId)
                    .SetProperty(game => game.Price, updateGame.Price)
                    .SetProperty(game => game.ReleaseDate, updateGame.ReleaseDate));
            if (rowsAffected == 0) return Results.NotFound();

            var updatedGame = await dbContext.Games.FindAsync(id);

            return Results.Ok(new GameDetailsDto(
                updatedGame!.Id,
                updatedGame.Name,
                updatedGame.GenreId,
                updatedGame.Price,
                updatedGame.ReleaseDate
            ));
        });



        // =============================
        // DELETE /games/1
        // =============================
        group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            await dbContext.Games.Where(game => game.Id == id).ExecuteDeleteAsync();
            return Results.NoContent();
        });

    }

}



// An endpoint is simply a door into your API — each door does something different depending on the HTTP method (GET, POST, PUT, DELETE).