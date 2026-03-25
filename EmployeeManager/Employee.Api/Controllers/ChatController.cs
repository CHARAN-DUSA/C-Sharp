using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Employee.Api.Model;        // ✅ IMPORTANT (fixes your errors)
using Microsoft.AspNetCore.Http;
using System.IO;

[Route("api/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly EmployeeDbContext db;
    private readonly IHubContext<ChatHub> hub;

    public ChatController(EmployeeDbContext db, IHubContext<ChatHub> hub)
    {
        this.db = db;
        this.hub = hub;
    }

    // ✅ SEND MESSAGE (TEXT + FILE)
    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] MessageModel model)
    {
        model.CreatedDate = DateTime.UtcNow;

        db.Messages.Add(model);
        await db.SaveChangesAsync();

        var senderName = await db.Employees
            .Where(e => e.EmployeeId == model.SenderId)
            .Select(e => e.Name)
            .FirstOrDefaultAsync();

        var dto = new ChatMessageDto
        {
            MessageId = model.MessageId,
            SenderId = model.SenderId,
            SenderName = senderName ?? "Unknown",
            ReceiverId = model.ReceiverId,
            MessageText = model.MessageText,
            FileUrl = model.FileUrl,     // ✅ FILE SUPPORT
            CreatedDate = model.CreatedDate
        };

        // 🔥 REAL-TIME BROADCAST
        await hub.Clients.All.SendAsync("ReceiveMessage", dto);

        return Ok(dto);
    }

    // ✅ GET MESSAGES
    [HttpGet("get/{userId}")]
    public async Task<IActionResult> GetMessages(int userId)
    {
        var data = await db.Messages
            .Include(m => m.Sender)
            .Where(m =>
                m.ReceiverId == null ||
                m.ReceiverId == userId ||
                m.SenderId == userId
            )
            .OrderBy(m => m.CreatedDate)
            .Select(m => new ChatMessageDto
            {
                MessageId = m.MessageId,
                SenderId = m.SenderId,
                SenderName = m.Sender != null ? m.Sender.Name : "Unknown", // ✅ safe
                ReceiverId = m.ReceiverId,
                MessageText = m.MessageText,
                FileUrl = m.FileUrl,   // ✅ FILE SUPPORT
                CreatedDate = m.CreatedDate
            })
            .ToListAsync();

        return Ok(data);
    }

    // ✅ FILE UPLOAD API
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";

        return Ok(new { fileUrl });
    }
}