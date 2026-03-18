using GameStore.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

public static class DataExtensions
{
    public static async Task InitializeDbAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope(); // Create a scope to get the DbContext
        {
            var context = scope.ServiceProvider.GetRequiredService<GameStoreContext>();
            await context.Database.MigrateAsync();
        }
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        var connString = configuration.GetConnectionString("GameStoreContext");
        services.AddSqlServer<GameStoreContext>(connString)
                .AddScoped<IGamesRepository, EntityFrameworkGamesRepository>();  // Here we use scoped lifetime for the repository, which the database is also created with.
        return services;
    }
}