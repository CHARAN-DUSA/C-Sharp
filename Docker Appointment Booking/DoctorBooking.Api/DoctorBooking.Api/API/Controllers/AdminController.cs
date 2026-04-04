using DoctorBooking.API.Application.DTOs.Common;
using DoctorBooking.API.Application.Interfaces;
using DoctorBooking.API.Domain.Entities;
using DoctorBooking.API.Domain.Enums;
using DoctorBooking.API.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoctorBooking.API.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _ctx;
    private readonly UserManager<AppUser> _userManager;
    private readonly IAppointmentRepository _apptRepo;
    private readonly IDoctorRepository _docRepo;
    private readonly IPatientRepository _patRepo;

    public AdminController(AppDbContext ctx, UserManager<AppUser> userManager,
        IAppointmentRepository apptRepo, IDoctorRepository docRepo, IPatientRepository patRepo)
    {
        _ctx = ctx;
        _userManager = userManager;
        _apptRepo = apptRepo;
        _docRepo = docRepo;
        _patRepo = patRepo;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var totalPatients = await _ctx.Patients.CountAsync();
        var totalDoctors = await _ctx.Doctors.CountAsync();
        var totalAppointments = await _ctx.Appointments.CountAsync();
        var todayAppointments = await _ctx.Appointments
            .CountAsync(a => a.AppointmentDate.Date == DateTime.UtcNow.Date);
        var totalRevenue = await _ctx.Appointments
            .Where(a => a.Status == AppointmentStatus.Completed)
            .SumAsync(a => a.ConsultationFee);
        var monthRevenue = await _ctx.Appointments
            .Where(a => a.Status == AppointmentStatus.Completed
                     && a.AppointmentDate.Month == DateTime.UtcNow.Month)
            .SumAsync(a => a.ConsultationFee);

        var byStatus = Enum.GetValues<AppointmentStatus>()
            .Select(s => new { status = s.ToString(), count = _ctx.Appointments.Count(a => a.Status == s) });

        return Ok(new
        {
            TotalPatients = totalPatients,
            TotalDoctors = totalDoctors,
            TotalAppointments = totalAppointments,
            TodayAppointments = todayAppointments,
            TotalRevenue = totalRevenue,
            MonthlyRevenue = monthRevenue,
            AppointmentsByStatus = byStatus
        });
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var total = await _userManager.Users.CountAsync();
        var users = await _userManager.Users
            .OrderByDescending(u => u.CreatedAt)
            .Skip((pageNumber - 1) * pageSize).Take(pageSize)
            .Select(u => new { u.Id, u.Email, u.FullName, u.Role, u.IsDeleted, u.CreatedAt })
            .ToListAsync();

        return Ok(new PaginatedResult<object>
        {
            Items = users.Cast<object>().ToList(),
            TotalCount = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
    }

    [HttpPatch("users/{userId}/toggle-status")]
    public async Task<IActionResult> ToggleUserStatus(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();
        user.IsDeleted = !user.IsDeleted;
        await _userManager.UpdateAsync(user);
        return Ok(new { isDeleted = user.IsDeleted });
    }

    [HttpGet("doctors")]
    public async Task<IActionResult> GetDoctors([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var total = await _ctx.Doctors.CountAsync();
        var doctors = await _ctx.Doctors
            .Include(d => d.User)
            .Skip((pageNumber - 1) * pageSize).Take(pageSize)
            .ToListAsync();

        return Ok(new PaginatedResult<object>
        {
            Items = doctors.Cast<object>().ToList(),
            TotalCount = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
    }

    [HttpPatch("doctors/{id}/verify")]
    public async Task<IActionResult> VerifyDoctor(int id)
    {
        var doc = await _ctx.Doctors.FindAsync(id);
        if (doc is null) return NotFound();
        doc.IsVerified = true;
        await _ctx.SaveChangesAsync();
        return Ok(new { message = "Doctor verified." });
    }

    [HttpGet("appointments")]
    public async Task<IActionResult> GetAllAppointments(
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20,
        [FromQuery] AppointmentStatus? status = null, [FromQuery] DateTime? date = null)
    {
        var total = await _apptRepo.GetTotalCountAsync(status, date);
        var items = await _apptRepo.GetAllAsync(pageNumber, pageSize, status, date);
        return Ok(new PaginatedResult<object>
        {
            Items = items.Cast<object>().ToList(),
            TotalCount = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
    }
}