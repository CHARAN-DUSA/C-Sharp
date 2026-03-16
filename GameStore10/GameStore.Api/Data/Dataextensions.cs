using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data
{
    public static class DataExtensions
    {
        public static void MigrateDb(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();  //using var means the scope is automatically destroyed when the method finishes
            var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();

            dbContext.Database.Migrate();
        }
        public static void AddGameStoreDb(this WebApplicationBuilder builder)
        {
            // Read the connection string from appsettings.json
            // instead of hardcoding "Data Source=GameStore.db" directly in code
            var connString = builder.Configuration.GetConnectionString("GameStore");
            // 1. It ensures that a new instance of GameStoreContext is created for each HTTP request
            // 2. Db Connections are a limited and expensive resource, so we want to create and dispose them as needed, rather than keeping a single instance alive for the entire app lifetime
            // 3. DbContext it not thread-safe. Scoped avoids to concurrency issues
            // 4. Makes it easier to manage transactions and ensure data consistency
            // 5. Reusing a DbContext instance across multiple requests can lead to increased memory usage 
            builder.Services.AddScoped<GameStoreContext>(); // Register GameStoreContext for dependency injection
            // Register GameStoreContext into the DI container ONCE
            // From now on, anywhere we need GameStoreContext, ASP.NET will create and give it to us automatically
            // We never need to write 'new GameStoreContext()' anywhere else in our app
            builder.Services.AddSqlite<GameStoreContext>(
                connString,

                // This runs once when the app starts to seed initial data
                optionsAction: options => options.UseSeeding((context, _) =>
                {
                    // Check if Genres table is empty — avoid adding duplicates every restart
                    if (!context.Set<Genre>().Any())
                    {
                        // Add default genres into the database
                        context.Set<Genre>().AddRange(
                            new Genre { Name = "Action" },
                            new Genre { Name = "Adventure" },
                            new Genre { Name = "RPG" },
                            new Genre { Name = "Strategy" },
                            new Genre { Name = "Sports" }
                        );

                        // Save changes to actually commit the data to the database
                        // Without this, nothing gets saved
                        context.SaveChanges();
                    }
                }));
        }
    }

}


// Migrations keep your C# models and database in sync, automatically.

// Instead of manually running dotnet ef database update, the database gets updated automatically every time the app starts. This is especially handy in development or containerized environments. The MigrateDb extension method creates a scope to get the GameStoreContext from the DI container, then calls Database.Migrate() to apply any pending migrations. This way, your database schema stays in sync with your EF Core models without extra manual steps.