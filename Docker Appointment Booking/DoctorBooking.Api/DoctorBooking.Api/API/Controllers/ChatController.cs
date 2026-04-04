using DoctorBooking.API.Application.DTOs.Chat;
using DoctorBooking.API.Application.Interfaces;
using DoctorBooking.API.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DoctorBooking.API.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatRepository _repo;
    public ChatController(IChatRepository repo) => _repo = repo;

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations()
        => Ok(await _repo.GetConversationListAsync(UserId));

    [HttpGet("messages/{participantId}")]
    public async Task<IActionResult> GetMessages(string participantId)
    {
        await _repo.MarkAsReadAsync(UserId, participantId);
        var messages = await _repo.GetConversationAsync(UserId, participantId);
        return Ok(messages.Select(m => new ChatMessageResponseDto
        {
            Id = m.Id,
            SenderId = m.SenderId,
            ReceiverId = m.ReceiverId,
            SenderName = m.Sender?.FullName ?? "",
            SenderAvatar = m.Sender?.ProfilePicture,
            Content = m.Content,
            SentAt = m.SentAt,
            IsRead = m.IsRead,
            AppointmentId = m.AppointmentId
        }));
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] SendMessageDto dto)
    {
        var message = new ChatMessage
        {
            SenderId = UserId,
            ReceiverId = dto.ReceiverId,
            Content = dto.Content,
            AppointmentId = dto.AppointmentId
        };
        var saved = await _repo.SaveMessageAsync(message);
        return Ok(new { id = saved.Id, sentAt = saved.SentAt });
    }

    [HttpPatch("read/{participantId}")]
    public async Task<IActionResult> MarkRead(string participantId)
    {
        await _repo.MarkAsReadAsync(UserId, participantId);
        return NoContent();
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> UnreadCount()
        => Ok(await _repo.GetUnreadCountAsync(UserId));
}