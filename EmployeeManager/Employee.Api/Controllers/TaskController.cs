using Employee.Api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Employee.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TaskController : ControllerBase
{
    private readonly EmployeeDbContext _db;
    public TaskController(EmployeeDbContext db) => _db = db;

    // GET api/Task — all tasks (HR view)
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await (
            from t in _db.Tasks
            join e in _db.Employees on t.AssignedToEmployeeId equals e.EmployeeId
            select new
            {
                t.TaskId,
                t.Title,
                t.Description,
                t.AssignedToEmployeeId,
                AssignedToName = e.Name,
                t.AssignedByEmployeeId,
                t.DueDate,
                t.Status,
                t.Priority,
                t.CreatedDate,
                t.CompletedDate,
                t.CompletionNote
            }
        ).OrderByDescending(t => t.CreatedDate).ToListAsync();

        return Ok(data);
    }

    // GET api/Task/employee/5 — tasks for one employee
    [HttpGet("employee/{employeeId}")]
    public async Task<IActionResult> GetByEmployee(int employeeId)
    {
        var data = await _db.Tasks
            .Where(t => t.AssignedToEmployeeId == employeeId)
            .OrderBy(t => t.DueDate)
            .ToListAsync();
        return Ok(data);
    }

    // GET api/Task/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task == null) return NotFound("Task not found.");
        return Ok(task);
    }

    // POST api/Task
    [HttpPost]
    public async Task<IActionResult> Create(TaskModel task)
    {
        task.CreatedDate = DateTime.Now;
        task.Status = "Pending";
        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = task.TaskId }, task);
    }

    // PUT api/Task/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, TaskModel task)
    {
        var existing = await _db.Tasks.FindAsync(id);
        if (existing == null) return NotFound("Task not found.");

        existing.Title = task.Title;
        existing.Description = task.Description;
        existing.AssignedToEmployeeId = task.AssignedToEmployeeId;
        existing.DueDate = task.DueDate;
        existing.Priority = task.Priority;
        existing.Status = task.Status;
        existing.CompletionNote = task.CompletionNote;

        if (task.Status == "Completed" && existing.CompletedDate == null)
            existing.CompletedDate = DateTime.Now;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // PATCH api/Task/5/status — employee marks own task
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task == null) return NotFound("Task not found.");

        task.Status = dto.Status;

        // ✅ Employee note update
        if (!string.IsNullOrEmpty(dto.CompletionNote))
        {
            task.CompletionNote = dto.CompletionNote;
        }

        // ✅ HR message update
        if (!string.IsNullOrEmpty(dto.HrMessage))
        {
            task.HrMessage = dto.HrMessage;
        }

        if (dto.Status == "Completed")
            task.CompletedDate = DateTime.Now;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/Task/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task == null) return NotFound("Task not found.");
        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

public class UpdateStatusDto
{
    public string Status { get; set; } = string.Empty;

    public string? CompletionNote { get; set; } // Employee
    public string? HrMessage { get; set; }      // HR
}