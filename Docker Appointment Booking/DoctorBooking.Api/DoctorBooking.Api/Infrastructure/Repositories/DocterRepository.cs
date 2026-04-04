using DoctorBooking.API.Application.Interfaces;
using DoctorBooking.API.Domain.Entities;
using DoctorBooking.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DoctorBooking.API.Infrastructure.Repositories;

public class DoctorRepository : IDoctorRepository
{
    private readonly AppDbContext _ctx;
    public DoctorRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task<Doctor?> GetByIdAsync(int id) =>
        await _ctx.Doctors
        .Include(d => d.User)
        .FirstOrDefaultAsync(d => d.Id == id);

    public async Task<Doctor?> GetByUserIdAsync(string userId) =>
        await _ctx.Doctors.
        Include(d => d.User).
        FirstOrDefaultAsync(d => d.UserId == userId);

    public Task<List<Doctor>> GetAllAsync(string? specialty = null, string? search = null) =>
    _ctx.Doctors
        .Include(d => d.User)
        .Where(d =>
            // Show both verified AND unverified doctors
            // Admin can verify later, but patients should still see them
            (specialty == null || d.Specialty == specialty) &&
            (search == null ||
             d.User.FirstName.Contains(search) ||
             d.User.LastName.Contains(search) ||
             d.Specialty.Contains(search)))
        .OrderByDescending(d => d.Rating)
        .ToListAsync();
    public async Task<Doctor> CreateAsync(Doctor doctor)
    {
        _ctx.Doctors.Add(doctor);
        await _ctx.SaveChangesAsync();
        return doctor;
    }

    public async Task UpdateAsync(Doctor doctor)
    {
        _ctx.Doctors.Update(doctor);
        await _ctx.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var d = await _ctx.Doctors.FindAsync(id);
        if (d is null) return;
        d.IsDeleted = true;
        await _ctx.SaveChangesAsync();
    }

    public Task<List<TimeSlot>> GetAvailableSlotsAsync(int doctorId, DateTime date) =>
        _ctx.TimeSlots
            .Where(t => t.DoctorId == doctorId && t.Date.Date == date.Date && !t.IsBooked && !t.IsBlocked)
            .OrderBy(t => t.StartTime)
            .ToListAsync();

    public async Task SetAvailabilityAsync(
    int doctorId, DateTime specificDate,
    string startTime, string endTime, int slotMinutes)
    {
        // Generate slots for ONE specific date only
        var existing = await _ctx.TimeSlots
            .Where(t => t.DoctorId == doctorId && t.Date.Date == specificDate.Date)
            .ToListAsync();

        // Remove only unbooked existing slots for that day
        var unbooked = existing.Where(t => !t.IsBooked).ToList();
        if (unbooked.Any()) _ctx.TimeSlots.RemoveRange(unbooked);

        var slots = new List<TimeSlot>();
        var current = TimeSpan.Parse(startTime);
        var end = TimeSpan.Parse(endTime);

        while (current + TimeSpan.FromMinutes(slotMinutes) <= end)
        {
            var slotStart = current.ToString(@"hh\:mm");
            var slotEnd = (current + TimeSpan.FromMinutes(slotMinutes)).ToString(@"hh\:mm");

            // Don't re-create already booked slots
            bool alreadyBooked = existing.Any(t => t.StartTime == slotStart && t.IsBooked);
            if (!alreadyBooked)
            {
                slots.Add(new TimeSlot
                {
                    DoctorId = doctorId,
                    Date = specificDate.Date,
                    StartTime = slotStart,
                    EndTime = slotEnd,
                    IsBooked = false,
                    IsBlocked = false
                });
            }
            current += TimeSpan.FromMinutes(slotMinutes);
        }

        _ctx.TimeSlots.AddRange(slots);
        await _ctx.SaveChangesAsync();
    }
    public async Task<double> GetAverageRatingAsync(int doctorId)
    {
        var reviews = await _ctx.Reviews.Where(r => r.DoctorId == doctorId).ToListAsync();
        return reviews.Any() ? reviews.Average(r => r.Rating) : 0;
    }

    public Task<List<string>> GetSpecialtiesAsync() =>
        _ctx.Doctors.Select(d => d.Specialty).Distinct().ToListAsync();

    public async Task<int> GetTotalPatientsAsync(int doctorId) =>
        await _ctx.Appointments.Where(a => a.DoctorId == doctorId).Select(a => a.PatientId).Distinct().CountAsync();
}