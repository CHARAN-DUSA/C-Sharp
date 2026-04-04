using DoctorBooking.API.Application.DTOs.Doctor;
using DoctorBooking.API.Application.Interfaces;
using DoctorBooking.API.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DoctorBooking.API.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    private readonly IDoctorRepository _repo;
    public DoctorsController(IDoctorRepository repo) => _repo = repo;

    // ── GET ALL DOCTORS ───────────────────────────────
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? specialty, [FromQuery] string? search)
    {
        var doctors = await _repo.GetAllAsync(specialty, search);

        var result = doctors.Select(doc => MapToDto(doc)).ToList();
        return Ok(result);
    }

    // ── GET BY ID ─────────────────────────────────────
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var doc = await _repo.GetByIdAsync(id);
        if (doc is null) return NotFound();

        return Ok(MapToDto(doc));
    }

    // ── GET SLOTS ─────────────────────────────────────
    [HttpGet("{id}/slots")]
    public async Task<IActionResult> GetSlots(int id, [FromQuery] DateTime date)
        => Ok(await _repo.GetAvailableSlotsAsync(id, date));

    // ── GET SPECIALTIES ───────────────────────────────
    [HttpGet("specialties")]
    public async Task<IActionResult> GetSpecialties()
        => Ok(await _repo.GetSpecialtiesAsync());

    // ── MY PROFILE ────────────────────────────────────
    [HttpGet("my-profile")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var doc = await _repo.GetByUserIdAsync(userId);
        if (doc is null) return NotFound("Doctor profile not found");

        return Ok(MapToDto(doc));
    }

    // ── UPDATE PROFILE ────────────────────────────────
    [HttpPut("my-profile")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] DoctorResponseDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var doc = await _repo.GetByUserIdAsync(userId);
        if (doc is null) return NotFound();

        doc.Bio = dto.Bio;
        doc.ConsultationFee = dto.ConsultationFee;
        doc.Languages = dto.Languages;
        doc.ClinicName = dto.ClinicName;
        doc.Address = dto.Address;

        await _repo.UpdateAsync(doc);

        return Ok(MapToDto(doc));
    }

    [HttpGet("my-slots")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> GetMySlots([FromQuery] DateTime date)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var doc = await _repo.GetByUserIdAsync(userId);
        if (doc is null) return NotFound();

        var slots = await _repo.GetAvailableSlotsAsync(doc.Id, date);

        var result = slots.Select(s => new
        {
            s.Id,
            s.Date,
            s.StartTime,
            s.EndTime,
            s.IsBooked,
            s.IsBlocked
        });

        return Ok(result);
    }
    // ── CANCEL A SLOT ─────────────────────────────────
    [HttpDelete("slots/{slotId}")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> CancelSlot(int slotId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var doc = await _repo.GetByUserIdAsync(userId);
        if (doc is null) return NotFound();

        var ctx = HttpContext.RequestServices.GetRequiredService<AppDbContext>();
        var slot = await ctx.TimeSlots.FirstOrDefaultAsync(s => s.Id == slotId && s.DoctorId == doc.Id);
        if (slot is null) return NotFound();

        if (slot.IsBooked)
            return BadRequest(new { detail = "Cannot cancel a booked slot." });

        ctx.TimeSlots.Remove(slot);
        await ctx.SaveChangesAsync();
        return NoContent();
    }

    // ── UPDATE A SLOT ─────────────────────────────────
    [HttpPatch("slots/{slotId}")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> UpdateSlot(int slotId, [FromBody] UpdateSlotDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var doc = await _repo.GetByUserIdAsync(userId);
        if (doc is null) return NotFound();

        var ctx = HttpContext.RequestServices.GetRequiredService<AppDbContext>();
        var slot = await ctx.TimeSlots.FirstOrDefaultAsync(s => s.Id == slotId && s.DoctorId == doc.Id);
        if (slot is null) return NotFound();

        if (slot.IsBooked)
            return BadRequest(new { detail = "Cannot adjust a booked slot." });

        slot.StartTime = dto.StartTime;
        slot.EndTime = dto.EndTime;
        await ctx.SaveChangesAsync();
        return Ok(slot);
    }

    // ── SET AVAILABILITY ──────────────────────────────
    [HttpPost("availability")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> SetAvailability([FromBody] DoctorAvailabilityDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var doc = await _repo.GetByUserIdAsync(userId);
        if (doc is null) return NotFound();

        await _repo.SetAvailabilityAsync(doc.Id, dto.Date, dto.StartTime, dto.EndTime, dto.SlotDurationMinutes);

        return Ok(new { message = $"Slots created for {dto.Date:MMM dd, yyyy}" });
    }

    // ── STATS ─────────────────────────────────────────
    [HttpGet("stats")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> GetStats()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var doc = await _repo.GetByUserIdAsync(userId);
        if (doc is null) return NotFound();

        return Ok(new
        {
            TotalPatients = await _repo.GetTotalPatientsAsync(doc.Id),
            Rating = await _repo.GetAverageRatingAsync(doc.Id),
            TotalReviews = TryParseInt(doc.TotalReviews)
        });
    }

    // ── MAPPING METHOD (VERY IMPORTANT) ───────────────
    private static DoctorResponseDto MapToDto(dynamic doc)
    {
        return new DoctorResponseDto
        {
            Id = doc.Id,
            UserId = doc.UserId,
            FirstName = doc.User?.FirstName ?? "",
            LastName = doc.User?.LastName ?? "",
            Email = doc.User?.Email ?? "",
            PhoneNumber = doc.User?.PhoneNumber,
            Specialty = doc.Specialty,
            Qualifications = doc.Qualifications,
            ConsultationFee = doc.ConsultationFee,
            Bio = doc.Bio,
            Languages = doc.Languages,
            ClinicName = doc.ClinicName,
            Address = doc.Address,
            Rating = doc.Rating,
            TotalReviews = TryParseInt(doc.TotalReviews),
            IsVerified = doc.IsVerified
        };
    }

    // ── SAFE PARSE ────────────────────────────────────
    private static int TryParseInt(object value)
    {
        return int.TryParse(value?.ToString(), out var result) ? result : 0;
    }
}