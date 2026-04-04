using DoctorBooking.API.Domain.Entities;

namespace DoctorBooking.API.Application.Interfaces;

public interface IPatientRepository
{
    Task<Patient?> GetByIdAsync(int id);
    Task<Patient?> GetByUserIdAsync(string userId);
    Task<List<Patient>> GetAllAsync(int page, int size);
    Task<int> GetTotalCountAsync();
    Task<Patient> CreateAsync(Patient patient);
    Task UpdateAsync(Patient patient);
    Task SoftDeleteAsync(int id);
}