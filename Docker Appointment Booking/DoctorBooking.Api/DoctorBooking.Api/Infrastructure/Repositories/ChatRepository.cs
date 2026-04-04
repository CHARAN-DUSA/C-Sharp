using DoctorBooking.API.Application.Interfaces;
using DoctorBooking.API.Domain.Entities;
using DoctorBooking.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DoctorBooking.API.Infrastructure.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly AppDbContext _ctx;
    public ChatRepository(AppDbContext ctx) => _ctx = ctx;

    public Task<List<ChatMessage>> GetConversationAsync(string userId, string participantId) =>
        _ctx.ChatMessages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Where(m => (m.SenderId == userId && m.ReceiverId == participantId) ||
                        (m.SenderId == participantId && m.ReceiverId == userId))
            .OrderBy(m => m.SentAt)
            .ToListAsync();

    public async Task<List<object>> GetConversationListAsync(string userId)
    {
        var messages = await _ctx.ChatMessages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Where(m => m.SenderId == userId || m.ReceiverId == userId)
            .OrderByDescending(m => m.SentAt)
            .ToListAsync();

        return messages
            .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
            .Select(g =>
            {
                var last = g.First();
                var other = last.SenderId == userId ? last.Receiver : last.Sender;
                var unread = g.Count(m => m.ReceiverId == userId && !m.IsRead);
                return (object)new
                {
                    ParticipantId = other.Id,
                    ParticipantName = other.FullName,
                    ParticipantAvatar = other.ProfilePicture,
                    LastMessage = last.Content,
                    LastMessageTime = last.SentAt,
                    UnreadCount = unread
                };
            })
            .ToList();
    }

    public async Task<ChatMessage> SaveMessageAsync(ChatMessage message)
    {
        _ctx.ChatMessages.Add(message);
        await _ctx.SaveChangesAsync();
        return message;
    }

    public async Task MarkAsReadAsync(string userId, string participantId)
    {
        var unread = await _ctx.ChatMessages
            .Where(m => m.SenderId == participantId && m.ReceiverId == userId && !m.IsRead)
            .ToListAsync();
        unread.ForEach(m => m.IsRead = true);
        await _ctx.SaveChangesAsync();
    }

    public Task<int> GetUnreadCountAsync(string userId) =>
        _ctx.ChatMessages.CountAsync(m => m.ReceiverId == userId && !m.IsRead);
}