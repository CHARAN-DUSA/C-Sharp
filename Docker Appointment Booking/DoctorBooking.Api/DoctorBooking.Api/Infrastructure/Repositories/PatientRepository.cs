using DoctorBooking.API.Application.Interfaces;
using DoctorBooking.API.Domain.Entities;
using DoctorBooking.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DoctorBooking.API.Infrastructure.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly AppDbContext _ctx;
    public PatientRepository(AppDbContext ctx) => _ctx = ctx;

    public Task<Patient?> GetByIdAsync(int id) =>
        _ctx.Patients.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id);

    public Task<Patient?> GetByUserIdAsync(string userId) =>
        _ctx.Patients.Include(p => p.User).FirstOrDefaultAsync(p => p.UserId == userId);

    public Task<List<Patient>> GetAllAsync(int page, int size) =>
        _ctx.Patients
            .Include(p => p.User)
            .OrderByDescending(p => p.Id)
            .Skip((page - 1) * size).Take(size)
            .ToListAsync();

    public Task<int> GetTotalCountAsync() => _ctx.Patients.CountAsync();

    public async Task<Patient> CreateAsync(Patient patient)
    {
        _ctx.Patients.Add(patient);
        await _ctx.SaveChangesAsync();
        return patient;
    }

    public async Task UpdateAsync(Patient patient)
    {
        _ctx.Patients.Update(patient);
        await _ctx.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var p = await _ctx.Patients.FindAsync(id);
        if (p is null) return;
        p.IsDeleted = true;
        await _ctx.SaveChangesAsync();
    }
}