using Employee.Api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Employee.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LeaveController : ControllerBase
{
    private readonly EmployeeDbContext _db;
    public LeaveController(EmployeeDbContext db) => _db = db;

    // GET api/Leave — all leaves (HR)
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await (
            from l in _db.Leaves
            join e in _db.Employees on l.EmployeeId equals e.EmployeeId
            select new
            {
                l.LeaveId,
                l.EmployeeId,
                EmployeeName = e.Name,
                l.LeaveType,
                l.FromDate,
                l.ToDate,
                l.TotalDays,
                l.Reason,
                l.Status,
                l.RejectionReason,
                l.ApprovedByEmployeeId,
                l.AppliedDate,
                l.ActionDate
            }
        ).OrderByDescending(l => l.AppliedDate).ToListAsync();

        return Ok(data);
    }

    // GET api/Leave/employee/5 — own leaves (employee)
    [HttpGet("employee/{employeeId}")]
    public async Task<IActionResult> GetByEmployee(int employeeId)
    {
        var data = await _db.Leaves
            .Where(l => l.EmployeeId == employeeId)
            .OrderByDescending(l => l.AppliedDate)
            .ToListAsync();
        return Ok(data);
    }

    // GET api/Leave/pending — pending leaves (HR)
    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        var data = await (
            from l in _db.Leaves
            join e in _db.Employees on l.EmployeeId equals e.EmployeeId
            where l.Status == "Pending"
            select new
            {
                l.LeaveId,
                l.EmployeeId,
                EmployeeName = e.Name,
                l.LeaveType,
                l.FromDate,
                l.ToDate,
                l.TotalDays,
                l.Reason,
                l.Status,
                l.AppliedDate
            }
        ).OrderByDescending(l => l.AppliedDate).ToListAsync();

        return Ok(data);
    }

    // POST api/Leave — apply for leave (employee)
    [HttpPost]
    public async Task<IActionResult> Apply(LeaveModel leave)
    {
        leave.TotalDays = (int)(leave.ToDate - leave.FromDate).TotalDays + 1;
        leave.AppliedDate = DateTime.Now;
        leave.Status = "Pending";

        _db.Leaves.Add(leave);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetByEmployee),
            new { employeeId = leave.EmployeeId }, leave);
    }

    // PATCH api/Leave/5/approve
    [HttpPatch("{id}/approve")]
    public async Task<IActionResult> Approve(int id, [FromBody] LeaveActionDto dto)
    {
        var leave = await _db.Leaves.FindAsync(id);
        if (leave == null) return NotFound();

        leave.Status = "Approved";
        leave.ApprovedByEmployeeId = dto.ActionByEmployeeId;
        leave.ActionDate = DateTime.Now;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // PATCH api/Leave/5/reject
    [HttpPatch("{id}/reject")]
    public async Task<IActionResult> Reject(int id, [FromBody] LeaveActionDto dto)
    {
        var leave = await _db.Leaves.FindAsync(id);
        if (leave == null) return NotFound();

        leave.Status = "Rejected";
        leave.RejectionReason = dto.Reason;
        leave.ApprovedByEmployeeId = dto.ActionByEmployeeId;
        leave.ActionDate = DateTime.Now;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/Leave/5 — cancel (employee, only if pending)
    [HttpDelete("{id}")]
    public async Task<IActionResult> Cancel(int id)
    {
        var leave = await _db.Leaves.FindAsync(id);
        if (leave == null) return NotFound();
        if (leave.Status != "Pending")
            return BadRequest("Only pending leaves can be cancelled.");

        _db.Leaves.Remove(leave);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

public class LeaveActionDto
{
    public int ActionByEmployeeId { get; set; }
    public string? Reason { get; set; }
}