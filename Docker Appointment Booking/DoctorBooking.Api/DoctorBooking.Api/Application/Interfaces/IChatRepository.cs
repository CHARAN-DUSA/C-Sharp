using DoctorBooking.API.Domain.Entities;

namespace DoctorBooking.API.Application.Interfaces;

public interface IChatRepository
{
    Task<List<ChatMessage>> GetConversationAsync(string userId, string participantId);
    Task<List<object>> GetConversationListAsync(string userId);
    Task<ChatMessage> SaveMessageAsync(ChatMessage message);
    Task MarkAsReadAsync(string userId, string participantId);
    Task<int> GetUnreadCountAsync(string userId);
}