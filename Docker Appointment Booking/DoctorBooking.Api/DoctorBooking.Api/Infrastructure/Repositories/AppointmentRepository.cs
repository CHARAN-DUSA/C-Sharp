using DoctorBooking.API.Application.Interfaces;
using DoctorBooking.API.Domain.Entities;
using DoctorBooking.API.Domain.Enums;
using DoctorBooking.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DoctorBooking.API.Infrastructure.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly AppDbContext _ctx;

    public AppointmentRepository(AppDbContext ctx)
    {
        _ctx = ctx;
    }

    public Task<Appointment?> GetByIdAsync(int id) =>
        _ctx.Appointments
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .FirstOrDefaultAsync(a => a.Id == id);

    public Task<List<Appointment>> GetByPatientIdAsync(int patientId, AppointmentStatus? status = null) =>
        _ctx.Appointments
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .Where(a => a.PatientId == patientId &&
                        (status == null || a.Status == status))
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();

    public Task<List<Appointment>> GetByDoctorIdAsync(int doctorId, AppointmentStatus? status = null) =>
        _ctx.Appointments
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .Where(a => a.DoctorId == doctorId &&
                        (status == null || a.Status == status))
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();

    public Task<List<Appointment>> GetAllAsync(int page, int size, AppointmentStatus? status, DateTime? date) =>
        _ctx.Appointments
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .Where(a =>
                (status == null || a.Status == status) &&
                (date == null || a.AppointmentDate.Date == date.Value.Date))
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

    public Task<int> GetTotalCountAsync(AppointmentStatus? status, DateTime? date) =>
        _ctx.Appointments
            .Where(a =>
                (status == null || a.Status == status) &&
                (date == null || a.AppointmentDate.Date == date.Value.Date))
            .CountAsync();

    public Task<bool> IsSlotTakenAsync(int doctorId, DateTime date, string timeSlot) =>
        _ctx.Appointments
            .AnyAsync(a =>
                a.DoctorId == doctorId &&
                a.AppointmentDate.Date == date.Date &&
                a.TimeSlot == timeSlot &&
                a.Status != AppointmentStatus.Cancelled);

    public async Task<Appointment> CreateAsync(Appointment appointment)
    {
        _ctx.Appointments.Add(appointment);
        await _ctx.SaveChangesAsync();
        return appointment;
    }

    // ✅ UPDATED (with CancellationToken)
    public async Task<bool> UpdateAsync(Appointment appointment, CancellationToken token)
    {
        try
        {
            appointment.UpdatedAt = DateTime.UtcNow;

            _ctx.Appointments.Update(appointment);
            await _ctx.SaveChangesAsync(token);

            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            return false; // concurrency conflict
        }
    }

    public async Task SoftDeleteAsync(int id)
    {
        var a = await _ctx.Appointments.FindAsync(id);
        if (a is null) return;

        a.IsDeleted = true;
        a.DeletedAt = DateTime.UtcNow;

        await _ctx.SaveChangesAsync();
    }

    // ✅ UPDATED (with CancellationToken)
    public Task<List<Appointment>> GetNoShowCandidatesAsync(CancellationToken token) =>
        _ctx.Appointments
            .Where(a =>
                a.Status == AppointmentStatus.Confirmed &&
                a.AppointmentDate < DateTime.UtcNow.AddHours(-2))
            .ToListAsync(token);

    public Task<int> CountByDoctorAsync(int doctorId, AppointmentStatus status) =>
        _ctx.Appointments
            .CountAsync(a => a.DoctorId == doctorId && a.Status == status);
}