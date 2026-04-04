using DoctorBooking.API.Domain.Entities;

namespace DoctorBooking.API.Application.Interfaces;

public interface INotificationRepository
{
    Task<List<Notification>> GetAllForUserAsync(string userId);
    Task<int> GetUnreadCountAsync(string userId);
    Task<Notification> CreateAsync(Notification notification);
    Task MarkReadAsync(int id);
    Task MarkAllReadAsync(string userId);
    Task SoftDeleteAsync(int id);
}