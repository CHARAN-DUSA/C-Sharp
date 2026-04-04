using DoctorBooking.API.Application.DTOs.Appointment;
using DoctorBooking.API.Application.Interfaces;
using DoctorBooking.API.Domain.Entities;
using DoctorBooking.API.Domain.Enums;
using DoctorBooking.API.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DoctorBooking.API.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentRepository _repo;
    private readonly IDoctorRepository _docRepo;
    private readonly IPatientRepository _patRepo;
    private readonly INotificationRepository _notifRepo;
    private readonly EmailService _email;

    public AppointmentsController(IAppointmentRepository repo, IDoctorRepository docRepo,
        IPatientRepository patRepo, INotificationRepository notifRepo, EmailService email)
    {
        _repo = repo;
        _docRepo = docRepo;
        _patRepo = patRepo;
        _notifRepo = notifRepo;
        _email = email;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // ✅ FIXED METHOD
    // GET api/appointments/my
    [HttpGet("my")]
    public async Task<IActionResult> GetMyAppointments([FromQuery] string? status)
    {
        // 🔥 Convert string → enum safely
        AppointmentStatus? parsedStatus = null;

        if (!string.IsNullOrEmpty(status) &&
            Enum.TryParse<AppointmentStatus>(status, true, out var result))
        {
            parsedStatus = result;
        }

        var role = User.FindFirstValue(ClaimTypes.Role);
        List<Appointment> appts;

        if (role == "Patient")
        {
            var patient = await _patRepo.GetByUserIdAsync(UserId);
            if (patient is null) return NotFound("Patient not found");

            appts = await _repo.GetByPatientIdAsync(patient.Id, parsedStatus);
        }
        else
        {
            var doctor = await _docRepo.GetByUserIdAsync(UserId);
            if (doctor is null) return NotFound("Doctor not found");

            appts = await _repo.GetByDoctorIdAsync(doctor.Id, parsedStatus);
        }

        return Ok(appts.Select(MapToDto));
    }

    // GET api/appointments/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var appt = await _repo.GetByIdAsync(id);
        return appt is null ? NotFound() : Ok(MapToDto(appt));
    }

    // POST api/appointments
    [HttpPost]
    [Authorize(Roles = "Patient")]
    public async Task<IActionResult> Book([FromBody] BookAppointmentDto dto)
    {
        var patient = await _patRepo.GetByUserIdAsync(UserId);
        if (patient is null) return BadRequest(new { detail = "Patient profile not found." });

        var doctor = await _docRepo.GetByIdAsync(dto.DoctorId);
        if (doctor is null) return NotFound(new { detail = "Doctor not found." });

        if (await _repo.IsSlotTakenAsync(dto.DoctorId, dto.AppointmentDate, dto.TimeSlot))
            return Conflict(new
            {
                title = "Slot Unavailable",
                detail = "This time slot was just taken by another patient. Please select another slot.",
                status = 409
            });

        var appt = new Appointment
        {
            PatientId = patient.Id,
            DoctorId = dto.DoctorId,
            AppointmentDate = dto.AppointmentDate,
            TimeSlot = dto.TimeSlot,
            Reason = dto.Reason,
            Status = AppointmentStatus.Confirmed,
            ConsultationFee = doctor.ConsultationFee
        };

        var created = await _repo.CreateAsync(appt);

        await _notifRepo.CreateAsync(new Notification
        {
            UserId = doctor.UserId,
            Title = "New Appointment",
            Message = $"New appointment from {patient.User.FullName} on {dto.AppointmentDate:MMM dd} at {dto.TimeSlot}",
            Type = "Appointment"
        });

        _ = Task.Run(() => _email.SendAppointmentConfirmationAsync(
            toEmail: patient.User.Email!,
            patientName: patient.User.FullName,
            doctorName: doctor.User.FullName,
            date: dto.AppointmentDate,
            timeSlot: dto.TimeSlot,
            fee: doctor.ConsultationFee,
            reason: dto.Reason));

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToDto(created));
    }

    // PATCH api/appointments/{id}/cancel
    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id, [FromBody] ConcurrencyDto dto)
    {
        var appt = await _repo.GetByIdAsync(id);
        if (appt is null) return NotFound();

        appt.RowVersion = Convert.FromBase64String(dto.RowVersion);
        appt.Status = AppointmentStatus.Cancelled;

        var success = await _repo.UpdateAsync(appt, HttpContext.RequestAborted);
        if (!success)
            return Conflict(new { detail = "Appointment was modified by someone else. Please refresh." });

        return NoContent();
    }

    // PATCH api/appointments/{id}/confirm
    [HttpPatch("{id}/confirm")]
    [Authorize(Roles = "Doctor,Admin")]
    public async Task<IActionResult> Confirm(int id, [FromBody] ConcurrencyDto dto)
    {
        var appt = await _repo.GetByIdAsync(id);
        if (appt is null) return NotFound();

        appt.RowVersion = Convert.FromBase64String(dto.RowVersion);
        appt.Status = AppointmentStatus.Confirmed;

        var success = await _repo.UpdateAsync(appt, HttpContext.RequestAborted);
        return success ? NoContent() : Conflict(new { detail = "Concurrency conflict. Please refresh." });
    }

    // PATCH api/appointments/{id}/complete
    [HttpPatch("{id}/complete")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> Complete(int id, [FromBody] CompleteAppointmentDto dto)
    {
        var appt = await _repo.GetByIdAsync(id);
        if (appt is null) return NotFound();

        appt.RowVersion = Convert.FromBase64String(dto.RowVersion);
        appt.Status = AppointmentStatus.Completed;
        appt.Prescription = dto.Prescription;
        appt.Notes = dto.Notes;

        var success = await _repo.UpdateAsync(appt, HttpContext.RequestAborted);
        return success ? NoContent() : Conflict(new { detail = "Concurrency conflict. Please refresh." });
    }

    private static AppointmentResponseDto MapToDto(Appointment a) => new()
    {
        Id = a.Id,
        PatientId = a.PatientId,
        DoctorId = a.DoctorId,
        PatientName = a.Patient?.User?.FullName ?? "",
        DoctorName = a.Doctor?.User?.FullName ?? "",
        DoctorSpecialty = a.Doctor?.Specialty ?? "",
        DoctorProfilePicture = a.Doctor?.User?.ProfilePicture,
        PatientProfilePicture = a.Patient?.User?.ProfilePicture,
        AppointmentDate = a.AppointmentDate,
        TimeSlot = a.TimeSlot,
        Status = a.Status,
        Reason = a.Reason,
        Notes = a.Notes,
        Prescription = a.Prescription,
        ConsultationFee = a.ConsultationFee,
        RowVersion = Convert.ToBase64String(a.RowVersion),
        CreatedAt = a.CreatedAt
    };
}

public record ConcurrencyDto(string RowVersion);
public record CompleteAppointmentDto(string RowVersion, string? Prescription, string? Notes);