using DoctorBooking.API.Application.DTOs.Review;
using DoctorBooking.API.Domain.Entities;
using DoctorBooking.API.Domain.Enums;
using DoctorBooking.API.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DoctorBooking.API.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly AppDbContext _db;
    public ReviewsController(AppDbContext db) => _db = db;

    // ── GET REVIEWS FOR A DOCTOR ─────────────────────────
    // GET /api/reviews/doctor/{doctorId}
    [HttpGet("doctor/{doctorId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDoctorReviews(int doctorId)
    {
        var reviews = await _db.Reviews
            .Include(r => r.Patient).ThenInclude(p => p.User)
            .Where(r => r.DoctorId == doctorId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewResponseDto(
                r.Id,
                r.Patient.User.FirstName + " " + r.Patient.User.LastName,
                r.Rating,
                r.Comment,
                r.CreatedAt
            ))
            .ToListAsync();

        return Ok(reviews);
    }

    // ── GET MY REVIEWS (as Patient) ──────────────────────
    // GET /api/reviews/my
    [HttpGet("my")]
    [Authorize(Roles = "Patient")]
    public async Task<IActionResult> GetMyReviews()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var patient = await _db.Patients
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (patient is null) return Ok(new List<ReviewResponseDto>());

        var reviews = await _db.Reviews
            .Include(r => r.Patient).ThenInclude(p => p.User)
            .Where(r => r.PatientId == patient.Id)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewResponseDto(
                r.Id,
                r.Patient.User.FirstName + " " + r.Patient.User.LastName,
                r.Rating,
                r.Comment,
                r.CreatedAt
            ))
            .ToListAsync();

        return Ok(reviews);
    }

    // ── CREATE REVIEW ────────────────────────────────────
    // POST /api/reviews
    [HttpPost]
    [Authorize(Roles = "Patient")]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        if (dto.Rating < 1 || dto.Rating > 5)
            return BadRequest(new { detail = "Rating must be between 1 and 5." });

        var patient = await _db.Patients
            .FirstOrDefaultAsync(p => p.UserId == userId);
        if (patient is null)
            return NotFound(new { detail = "❌ Patient profile not found." });

        // ✅ Debug — confirm what DoctorId is arriving
        var doctor = await _db.Doctors.FindAsync(dto.DoctorId);
        if (doctor is null)
            return NotFound(new { detail = $"❌ Doctor not found. Received DoctorId = {dto.DoctorId}" });

        var appointment = await _db.Appointments
            .FirstOrDefaultAsync(a =>
                a.PatientId == patient.Id &&
                a.DoctorId == dto.DoctorId &&
                a.Status == AppointmentStatus.Completed);

        if (appointment is null)
            return BadRequest(new
            {
                detail = "❌ No completed appointment found.",
                patientId = patient.Id,
                doctorId = dto.DoctorId
            });

        var exists = await _db.Reviews
            .AnyAsync(r => r.PatientId == patient.Id && r.DoctorId == dto.DoctorId);
        if (exists)
            return Conflict(new { detail = "You have already reviewed this doctor." });

        var review = new Review
        {
            PatientId = patient.Id,
            DoctorId = dto.DoctorId,
            AppointmentId = appointment.Id,
            Rating = dto.Rating,
            Comment = dto.Comment ?? "",
            CreatedAt = DateTime.UtcNow
        };

        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        var avgRating = await _db.Reviews
            .Where(r => r.DoctorId == dto.DoctorId)
            .AverageAsync(r => (double)r.Rating);

        doctor.Rating = avgRating;
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMyReviews), new { detail = "Review submitted." });
    }
    // ── DELETE REVIEW ────────────────────────────────────
    // DELETE /api/reviews/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Patient,Admin")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var review = await _db.Reviews
            .Include(r => r.Patient)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (review is null) return NotFound();

        // Only owner or Admin can delete
        var isAdmin = User.IsInRole("Admin");
        if (!isAdmin && review.Patient.UserId != userId)
            return Forbid();

        review.IsDeleted = true; // soft delete
        await _db.SaveChangesAsync();

        return Ok(new { detail = "Review deleted." });
    }
}