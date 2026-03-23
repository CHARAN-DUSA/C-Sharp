using Employee.Api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Employee.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AttendanceController : ControllerBase
{
    private readonly EmployeeDbContext _db;
    public AttendanceController(EmployeeDbContext db) => _db = db;

    // GET api/Attendance — all attendance (HR monitoring)
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? month,
        [FromQuery] int? year)
    {
        var m = month ?? DateTime.Now.Month;
        var y = year ?? DateTime.Now.Year;

        var data = await (
            from a in _db.Attendances
            join e in _db.Employees on a.EmployeeId equals e.EmployeeId
            where a.Date.Month == m && a.Date.Year == y
            select new
            {
                a.AttendanceId,
                a.EmployeeId,
                EmployeeName = e.Name,
                a.Date,
                a.CheckIn,
                a.CheckOut,
                a.Status,
                a.WorkingHours,
                a.Notes
            }
        ).OrderByDescending(a => a.Date)
         .ThenBy(a => a.EmployeeName)
         .ToListAsync();

        return Ok(data);
    }

    // GET api/Attendance/employee/5 — own attendance
    [HttpGet("employee/{employeeId}")]
    public async Task<IActionResult> GetByEmployee(
        int employeeId,
        [FromQuery] int? month,
        [FromQuery] int? year)
    {
        var m = month ?? DateTime.Now.Month;
        var y = year ?? DateTime.Now.Year;

        var data = await _db.Attendances
            .Where(a => a.EmployeeId == employeeId
                     && a.Date.Month == m
                     && a.Date.Year == y)
            .OrderBy(a => a.Date)
            .ToListAsync();

        return Ok(data);
    }

    // GET api/Attendance/summary/5 — monthly summary
    [HttpGet("summary/{employeeId}")]
    public async Task<IActionResult> GetSummary(
        int employeeId,
        [FromQuery] int? month,
        [FromQuery] int? year)
    {
        var m = month ?? DateTime.Now.Month;
        var y = year ?? DateTime.Now.Year;

        var records = await _db.Attendances
            .Where(a => a.EmployeeId == employeeId
                     && a.Date.Month == m
                     && a.Date.Year == y)
            .ToListAsync();

        return Ok(new
        {
            Present = records.Count(a => a.Status == "Present"),
            Absent = records.Count(a => a.Status == "Absent"),
            HalfDay = records.Count(a => a.Status == "HalfDay"),
            OnLeave = records.Count(a => a.Status == "Leave"),
            Total = records.Count,
            TotalWorkingHours = records.Sum(a => a.WorkingHours ?? 0)
        });
    }

    // POST api/Attendance/checkin — employee checks in
    [HttpPost("checkin")]
    public async Task<IActionResult> CheckIn([FromBody] CheckInDto dto)
    {
        var today = DateTime.Today;

        bool exists = await _db.Attendances
            .AnyAsync(a => a.EmployeeId == dto.EmployeeId
                        && a.Date == today);
        if (exists)
            return BadRequest("Already checked in today.");

        var attendance = new AttendanceModel
        {
            EmployeeId = dto.EmployeeId,
            Date = today,
            CheckIn = DateTime.Now.TimeOfDay,
            Status = "Present",
            Notes = dto.Notes
        };

        _db.Attendances.Add(attendance);
        await _db.SaveChangesAsync();
        return Ok(attendance);
    }

    // PATCH api/Attendance/checkout — employee checks out
    [HttpPatch("checkout")]
    public async Task<IActionResult> CheckOut([FromBody] CheckOutDto dto)
    {
        var today = DateTime.Today;

        var attendance = await _db.Attendances
            .FirstOrDefaultAsync(a => a.EmployeeId == dto.EmployeeId
                                   && a.Date == today);
        if (attendance == null)
            return NotFound("No check-in found for today.");
        if (attendance.CheckOut != null)
            return BadRequest("Already checked out today.");

        attendance.CheckOut = DateTime.Now.TimeOfDay;
        attendance.WorkingHours = (attendance.CheckOut - attendance.CheckIn)
                                  ?.TotalHours;
        attendance.Status = attendance.WorkingHours < 4 ? "HalfDay" : "Present";

        await _db.SaveChangesAsync();
        return Ok(attendance);
    }

    // GET api/Attendance/today/5 — check today's status
    [HttpGet("today/{employeeId}")]
    public async Task<IActionResult> GetToday(int employeeId)
    {
        var today = DateTime.Today;
        var record = await _db.Attendances
            .FirstOrDefaultAsync(a => a.EmployeeId == employeeId
                                   && a.Date == today);
        return Ok(record);
    }

    // POST api/Attendance/manual — HR adds attendance manually
    [HttpPost("manual")]
    public async Task<IActionResult> AddManual(AttendanceModel attendance)
    {
        bool exists = await _db.Attendances
            .AnyAsync(a => a.EmployeeId == attendance.EmployeeId
                        && a.Date == attendance.Date.Date);
        if (exists)
            return BadRequest("Attendance for this date already exists.");

        if (attendance.CheckIn.HasValue && attendance.CheckOut.HasValue)
            attendance.WorkingHours =
                (attendance.CheckOut - attendance.CheckIn)?.TotalHours;

        attendance.Date = attendance.Date.Date;
        _db.Attendances.Add(attendance);
        await _db.SaveChangesAsync();
        return Ok(attendance);
    }
}

public class CheckInDto
{
    public int EmployeeId { get; set; }
    public string? Notes { get; set; }
}

public class CheckOutDto
{
    public int EmployeeId { get; set; }
}