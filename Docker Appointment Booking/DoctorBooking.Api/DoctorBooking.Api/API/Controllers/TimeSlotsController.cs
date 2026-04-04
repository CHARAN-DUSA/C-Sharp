//using DoctorBooking.Api.Application.DTOs.Docter;
//using DoctorBooking.Api.Application.DTOs.Review;
//using DoctorBooking.Api.Application.Interfaces;
//using DoctorBooking.Api.Domain.Entities;
//using DoctorBooking.Api.Infrastructure.Data;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Security.Claims;

//namespace DoctorBooking.Api.API.Controllers;

//[ApiController]
//[Route("api/[controller]")]
//public class DoctorsController : ControllerBase
//{
//    private readonly IDoctorRepository _repo;
//    private readonly AppDbContext _db;

//    public DoctorsController(IDoctorRepository repo, AppDbContext db)
//    {
//        _repo = repo;
//        _db = db;
//    }

//    [HttpGet]
//    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
//        => Ok(await _repo.GetAllAsync(page, pageSize));

//    [HttpGet("{id}")]
//    public async Task<IActionResult> GetById(int id)
//    {
//        var doc = await _repo.GetByIdAsync(id);
//        return doc == null ? NotFound() : Ok(doc);
//    }

//    [HttpGet("{id}/availability")]
//    public async Task<IActionResult> GetAvailability(int id, [FromQuery] DateTime date)
//        => Ok(await _repo.GetAvailabilityAsync(id, date));

//    [HttpGet("{id}/reviews")]
//    public async Task<IActionResult> GetReviews(int id)
//    {
//        var reviews = await _db.Reviews
//            .Include(r => r.Patient)
//            .ThenInclude(p => p.User)
//            .Where(r => r.DoctorId == id)
//            .Select(r => new ReviewResponseDto
//            {
//                Id = r.Id,
//                PatientName = r.Patient.User.FullName,
//                Rating = r.Rating,
//                Comment = r.Comment,
//                CreatedAt = r.CreatedAt
//            })
//            .ToListAsync();

//        return Ok(reviews);
//    }

//    [HttpPost("{id}/timeslots")]
//    [Authorize(Roles = "Doctor")]
//    public async Task<IActionResult> AddTimeSlot(int id, [FromBody] DoctorAvailabilityDto dto)
//    {
//        var slot = new TimeSlot
//        {
//            DoctorId = id,
//            StartTime = dto.StartTime,
//            EndTime = dto.EndTime
//        };

//        _db.TimeSlots.Add(slot);
//        await _db.SaveChangesAsync();

//        return Ok(slot);
//    }

//    [HttpPost("{id}/reviews")]
//    [Authorize(Roles = "Patient")]
//    public async Task<IActionResult> AddReview(int id, [FromBody] CreateReviewDto dto)
//    {
//        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

//        var patient = await _db.Patients
//            .FirstOrDefaultAsync(p => p.UserId == userId);

//        if (patient == null)
//            return BadRequest();

//        _db.Reviews.Add(new Review
//        {
//            DoctorId = id,
//            PatientId = patient.Id,
//            Rating = dto.Rating,
//            Comment = dto.Comment
//        });

//        await _db.SaveChangesAsync();

//        return Ok();
//    }
//}