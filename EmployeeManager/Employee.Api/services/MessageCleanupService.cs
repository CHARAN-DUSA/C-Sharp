using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Employee.Api.Model; // for DbContext
using Microsoft.Extensions.DependencyInjection;

namespace Employee.Api.Services;   // ✅ IMPORTANT

public class MessageCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public MessageCleanupService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();

            var expiry = DateTime.UtcNow.AddHours(-24);

            var oldMessages = await context.Messages
                .Where(m => m.CreatedDate < expiry)
                .ToListAsync();

            if (oldMessages.Any())
            {
                context.Messages.RemoveRange(oldMessages);
                await context.SaveChangesAsync();
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}