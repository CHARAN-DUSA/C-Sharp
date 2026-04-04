using DoctorBooking.API.Application.Interfaces;
using DoctorBooking.API.Domain.Entities;
using DoctorBooking.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DoctorBooking.API.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _ctx;
    public NotificationRepository(AppDbContext ctx) => _ctx = ctx;

    public Task<List<Notification>> GetAllForUserAsync(string userId) =>
        _ctx.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

    public Task<int> GetUnreadCountAsync(string userId) =>
        _ctx.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);

    public async Task<Notification> CreateAsync(Notification notification)
    {
        _ctx.Notifications.Add(notification);
        await _ctx.SaveChangesAsync();
        return notification;
    }

    public async Task MarkReadAsync(int id)
    {
        var n = await _ctx.Notifications.FindAsync(id);
        if (n is null) return;
        n.IsRead = true;
        await _ctx.SaveChangesAsync();
    }

    public async Task MarkAllReadAsync(string userId)
    {
        var all = await _ctx.Notifications.Where(n => n.UserId == userId && !n.IsRead).ToListAsync();
        all.ForEach(n => n.IsRead = true);
        await _ctx.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var n = await _ctx.Notifications.FindAsync(id);
        if (n is null) return;
        n.IsDeleted = true;
        await _ctx.SaveChangesAsync();
    }
}