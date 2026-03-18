using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data
{
    public class GameStoreContext(DbContextOptions<GameStoreContext> options) : DbContext(options)  // HEre DbContext can act as a repository without need of writing a repository pattern.
        //Initially GameStoreContext doesn't know to store and use which database thats why DbContextOptions search for option like sqlite,postgresql of type gamestorecontext later it uses the method which mentioned in the program.cs
        //GameStoreContext inherits DbContext to get all EF Core database powers — then just adds your tables on top (Games, Genres). And perform on Gamestore.db whcih is a database we are created and the ooperations are performed in the DbContect Ef core without even creating directly using
        //The<GameStoreContext> part is important too — it makes sure it picks up only the options meant for GameStoreContext, not some other context if you had multiple.
    {
        public DbSet<Game> Games => Set<Game>();
        public DbSet<Genre> Genres { get; set; }
    }
}
