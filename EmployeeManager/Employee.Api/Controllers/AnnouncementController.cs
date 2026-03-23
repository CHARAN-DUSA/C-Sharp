using Employee.Api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Employee.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AnnouncementController : ControllerBase
{
    private readonly EmployeeDbContext _db;
    public AnnouncementController(EmployeeDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _db.Announcements
            .Where(a => a.IsActive)
            .OrderByDescending(a => a.CreatedDate)
            .ToListAsync();
        return Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var a = await _db.Announcements.FindAsync(id);
        if (a == null) return NotFound();
        return Ok(a);
    }

    [HttpPost]
    public async Task<IActionResult> Create(AnnouncementModel announcement)
    {
        announcement.CreatedDate = DateTime.Now;
        _db.Announcements.Add(announcement);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = announcement.AnnouncementId }, announcement);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, AnnouncementModel announcement)
    {
        var existing = await _db.Announcements.FindAsync(id);
        if (existing == null) return NotFound();
        existing.Title = announcement.Title;
        existing.Content = announcement.Content;
        existing.TargetRole = announcement.TargetRole;
        existing.IsActive = announcement.IsActive;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var a = await _db.Announcements.FindAsync(id);
        if (a == null) return NotFound();
        a.IsActive = false; // soft delete
        await _db.SaveChangesAsync();
        return NoContent();
    }
}