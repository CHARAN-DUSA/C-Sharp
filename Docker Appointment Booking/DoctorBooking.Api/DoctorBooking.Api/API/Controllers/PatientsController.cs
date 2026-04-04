// PatientsController.cs
using DoctorBooking.API.Application.Interfaces;
using DoctorBooking.API.Domain.Entities;
using DoctorBooking.API.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DoctorBooking.API.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Patient,Admin")]
public class PatientsController : ControllerBase
{
    private readonly IPatientRepository _repo;
    private readonly UserManager<AppUser> _userManager;
    private readonly AppDbContext _db;

    public PatientsController(
        IPatientRepository repo,
        UserManager<AppUser> userManager,
        AppDbContext db)
    {
        _repo = repo;
        _userManager = userManager;
        _db = db;
    }

    // ── GET MY PROFILE ───────────────────────────────────
    [HttpGet("my-profile")]
    [Authorize(Roles = "Patient")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        try
        {
            // Include User to avoid serialization crash (500 fix)
            var patient = await _db.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient is null) return NotFound(new { detail = "Patient profile not found." });

            return Ok(new
            {
                patient.Id,
                patient.UserId,
                FirstName = patient.User?.FirstName ?? "",
                LastName = patient.User?.LastName ?? "",
                Email = patient.User?.Email ?? "",
                PhoneNumber = patient.User?.PhoneNumber,
                patient.DateOfBirth,
                patient.Gender,
                patient.BloodGroup,
                patient.Allergies,
                patient.MedicalHistory,
                patient.EmergencyContact,
                patient.Address
            });
        }
        catch (Exception ex)
        {
            // Log real error to backend terminal for debugging
            Console.WriteLine($"ERROR GetMyProfile: {ex.Message}");
            return StatusCode(500, new { detail = "An error occurred fetching your profile." });
        }
    }

    // ── UPDATE MY PROFILE ────────────────────────────────
    [HttpPut("my-profile")]
    [Authorize(Roles = "Patient")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdatePatientDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var patient = await _repo.GetByUserIdAsync(userId);
        if (patient is null) return NotFound(new { detail = "Patient profile not found." });

        patient.Gender = dto.Gender ?? patient.Gender;
        patient.BloodGroup = dto.BloodGroup ?? patient.BloodGroup;
        patient.Allergies = dto.Allergies ?? patient.Allergies;
        patient.MedicalHistory = dto.MedicalHistory ?? patient.MedicalHistory;
        patient.EmergencyContact = dto.EmergencyContact ?? patient.EmergencyContact;
        patient.Address = dto.Address ?? patient.Address;

        var user = await _userManager.FindByIdAsync(userId);
        if (user is not null)
        {
            user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;
            await _userManager.UpdateAsync(user);
        }

        await _repo.UpdateAsync(patient);
        return Ok(new { detail = "Profile updated successfully." });
    }
}

public record UpdatePatientDto(
    string? Gender, string? BloodGroup, string? Allergies,
    string? MedicalHistory, string? EmergencyContact,
    string? Address, string? PhoneNumber);