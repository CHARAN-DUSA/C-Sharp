using DoctorBooking.API.Application.Interfaces;
using DoctorBooking.API.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace DoctorBooking.API.API.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IChatRepository _repo;
    private static readonly Dictionary<string, string> _connections = new();

    public ChatHub(IChatRepository repo) => _repo = repo;

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!;
        _connections[userId] = Context.ConnectionId;
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!;
        _connections.Remove(userId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string receiverId, string content, int? appointmentId = null)
    {
        var senderId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var senderName = Context.User?.FindFirstValue(ClaimTypes.Name)
            ?? throw new UnauthorizedAccessException("User not authenticated");
        var message = new ChatMessage
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Content = content,
            AppointmentId = appointmentId
        };
        var saved = await _repo.SaveMessageAsync(message);

        var payload = new
        {
            id = saved.Id,
            senderId = senderId,
            receiverId = receiverId,
            senderName = senderName,
            content = content,
            sentAt = saved.SentAt,
            isRead = false
        };

        // Push to receiver's group
        await Clients.Group($"user_{receiverId}").SendAsync("ReceiveMessage", payload);
        // Echo back to sender
        await Clients.Caller.SendAsync("MessageSent", payload);
    }

    public async Task MarkRead(string participantId)
    {
        var userId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _repo.MarkAsReadAsync(userId, participantId);
    }
}