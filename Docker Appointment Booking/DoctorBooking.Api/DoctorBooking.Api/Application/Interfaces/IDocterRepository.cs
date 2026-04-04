using DoctorBooking.API.Domain.Entities;

namespace DoctorBooking.API.Application.Interfaces;

public interface IDoctorRepository
{
    Task<Doctor?> GetByIdAsync(int id);
    Task<Doctor?> GetByUserIdAsync(string userId);
    Task<List<Doctor>> GetAllAsync(string? specialty = null, string? search = null);
    Task<Doctor> CreateAsync(Doctor doctor);
    Task UpdateAsync(Doctor doctor);
    Task SoftDeleteAsync(int id);
    Task<List<TimeSlot>> GetAvailableSlotsAsync(int doctorId, DateTime date);
    Task SetAvailabilityAsync(int doctorId, DateTime date, string startTime, string endTime, int slotMinutes); Task<double> GetAverageRatingAsync(int doctorId);
    Task<List<string>> GetSpecialtiesAsync();
    Task<int> GetTotalPatientsAsync(int doctorId);

}