using DoctorBooking.API.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoctorBooking.Api.API.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    // ✅ Stats (Doctors, Patients, Rating)
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var doctors = await _context.Doctors.CountAsync();
        var patients = await _context.Users.CountAsync(u => u.Role == "Patient");

        var avgRating = await _context.Doctors.AnyAsync()
            ? await _context.Doctors.AverageAsync(d => d.Rating)
            : 0;

        return Ok(new
        {
            doctors,
            patients,
            rating = Math.Round(avgRating, 1)
        });
    }

    // ✅ Specialties (dynamic)
    [HttpGet("specialties")]
    public async Task<IActionResult> GetSpecialties()
    {
        var data = await _context.Doctors
            .GroupBy(d => d.Specialty)
            .Select(g => new
            {
                name = g.Key,
                count = g.Count()
            })
            .ToListAsync();

        return Ok(data);
    }
}