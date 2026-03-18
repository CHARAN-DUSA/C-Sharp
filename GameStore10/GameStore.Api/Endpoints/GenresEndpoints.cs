using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints
{
    public static class GenresEndpoints
    {
        public static void MapGenresEndpoints(this WebApplication app) 
        {
            var group = app.MapGroup("/genres");

            // GET /genres
            group.MapGet("/", async (GameStoreContext dbContext) =>
                await dbContext.Genres
                                .Select(genre => new GenreDto(genre.Id, genre.Name))
                                .AsNoTracking()
                                .ToListAsync());

            // GET /genres/{id}
            group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
            {
                var genre = await dbContext.Genres.FindAsync(id);
                return genre is null ? Results.NotFound() : Results.Ok(new GenreDto(genre.Id, genre.Name));
            });

            // POST /genres
            group.MapPost("/", async (CreateGenreDto newGenre, GameStoreContext dbContext) =>
            {
                var genre = new Genre
                {
                    Name = newGenre.Name
                };

                dbContext.Genres.Add(genre);
                await dbContext.SaveChangesAsync();

                return Results.Created($"/genres/{genre.Id}", new GenreDto(genre.Id, genre.Name));
            });

            // PUT /genres/{id}
            group.MapPut("/{id}", async (int id, UpdateGenreDto updatedGenre, GameStoreContext dbContext) =>
            {
                var genre = await dbContext.Genres.FindAsync(id);
                if (genre is null) return Results.NotFound();

                genre.Name = updatedGenre.Name;
                await dbContext.SaveChangesAsync();

                return Results.NoContent();
            });

            // DELETE /genres/{id}
            group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
            {
                var genre = await dbContext.Genres.FindAsync(id);
                if (genre is null) return Results.NotFound();

                dbContext.Genres.Remove(genre);
                await dbContext.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}