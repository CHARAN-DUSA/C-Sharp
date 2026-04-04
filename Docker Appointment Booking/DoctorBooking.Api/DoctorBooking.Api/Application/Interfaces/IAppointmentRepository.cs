using DoctorBooking.API.Domain.Entities;
using DoctorBooking.API.Domain.Enums;

namespace DoctorBooking.API.Application.Interfaces;

public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(int id);

    Task<List<Appointment>> GetByPatientIdAsync(int patientId, AppointmentStatus? status = null);

    Task<List<Appointment>> GetByDoctorIdAsync(int doctorId, AppointmentStatus? status = null);

    Task<List<Appointment>> GetAllAsync(int page, int size, AppointmentStatus? status, DateTime? date);

    Task<int> GetTotalCountAsync(AppointmentStatus? status, DateTime? date);

    Task<bool> IsSlotTakenAsync(int doctorId, DateTime date, string timeSlot);

    Task<Appointment> CreateAsync(Appointment appointment);

    // ✅ UPDATED → supports cancellation
    Task<bool> UpdateAsync(Appointment appointment, CancellationToken token);

    Task SoftDeleteAsync(int id);

    // ✅ UPDATED → supports cancellation
    Task<List<Appointment>> GetNoShowCandidatesAsync(CancellationToken token);

    Task<int> CountByDoctorAsync(int doctorId, AppointmentStatus status);
}