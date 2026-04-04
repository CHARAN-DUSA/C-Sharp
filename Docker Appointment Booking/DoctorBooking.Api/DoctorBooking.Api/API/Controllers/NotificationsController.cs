using DoctorBooking.API.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DoctorBooking.API.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationRepository _repo;
    public NotificationsController(INotificationRepository repo) => _repo = repo;

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _repo.GetAllForUserAsync(UserId));

    [HttpGet("unread-count")]
    public async Task<IActionResult> UnreadCount()
        => Ok(await _repo.GetUnreadCountAsync(UserId));

    [HttpPatch("{id}/read")]
    public async Task<IActionResult> MarkRead(int id)
    {
        await _repo.MarkReadAsync(id);
        return NoContent();
    }

    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllRead()
    {
        await _repo.MarkAllReadAsync(UserId);
        return NoContent();
    }
}