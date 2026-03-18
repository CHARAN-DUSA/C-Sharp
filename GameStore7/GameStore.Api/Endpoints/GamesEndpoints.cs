using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    // Named route used in CreatedAtRoute to point back to GET /games/{id}
    const string GetGameEndpointName = "GetGame";

    public static RouteGroupBuilder MapGamesEndpoints(
        this IEndpointRouteBuilder routes)
    {

        // All routes in this group start with /games
        // Incoming request data is automatically validated
        var group = routes.MapGroup("/games")
                          .WithParameterValidation();



        // =============================
        // GET /games
        // =============================
        // Returns all games from the repository
        group.MapGet("/", async (IGamesRepository repository) =>
            (await repository.GetAllAsync()).Select(game => game.AsDto()));  // By using the AsDto extension method, we can convert our Game entities to GameDto objects before returning them in the response. This way, we can control exactly what data is sent back to the client and avoid exposing any internal details of our entities.



        // =============================
        // Get /games/{id}
        // =============================
        // Returns a single game by Id
        // 200 with game data if found, 404 if not found
        group.MapGet("/{id}", async (IGamesRepository repository, int id) =>
        {
            Game? game = await repository.GetAsync(id);

            return game is not null
                ? Results.Ok(game.AsDto())
                : Results.NotFound();
        })
        .WithName(GetGameEndpointName);



        // =============================
        // POST /games
        // =============================
        // Adds a new game to the repository
        // Returns 201 with location header pointing to GET /games/{id}
        group.MapPost("/", (IGamesRepository repository, CreateGameDto gameDto) =>
        {
            Game game = new()
            {
                Name = gameDto.Name,
                Genre = gameDto.Genre,
                Price = gameDto.Price,
                ReleaseDate = gameDto.ReleaseDate,
                ImageUri = gameDto.ImageUri
            };

            game.Id = repository.CreateAsync(game).Id;

            return Results.CreatedAtRoute(
                GetGameEndpointName,
                new { id = game.Id },
                game);
        });



        // =============================
        // PUT/ games/1
        // =============================
        // Updates an existing game by Id
        // Returns 404 if game not found, 200 with updated game if successful
        group.MapPut("/{id}", async (IGamesRepository repository, int id, UpdateGameDto updatedGameDto) =>
        {
            var game = await repository.GetAsync(id);
            if (game is null) return Results.NotFound();

            game.Name = updatedGameDto.Name;
            game.Genre = updatedGameDto.Genre;
            game.Price = updatedGameDto.Price;
            game.ReleaseDate = updatedGameDto.ReleaseDate;
            game.ImageUri = updatedGameDto.ImageUri;

            await repository.UpdateAsync(id, game);
            return Results.Ok(game);
        });



        // =============================
        // DELETE /games/1
        // =============================
        // Removes a game by Id from the repository
        // Returns 404 if game not found, 200 if deleted successfully
        group.MapDelete("/{id}", (IGamesRepository repository, int id) =>
        {
            var game = repository.GetAsync(id);
            if (game is null) return Results.NotFound();

            repository.DeleteAsync(id);
            return Results.Ok("Game deleted successfully.");
        });

        // Returns group back to Program.cs for further chaining
        return group;
    }
}