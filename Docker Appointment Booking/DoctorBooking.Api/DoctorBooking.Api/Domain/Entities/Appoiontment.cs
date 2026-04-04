using DoctorBooking.API.Domain.Enums;

namespace DoctorBooking.API.Domain.Entities;

public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string TimeSlot { get; set; } = string.Empty;
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string? Prescription { get; set; }
    public DateTime? FollowUpDate { get; set; }
    public decimal ConsultationFee { get; set; }
    public bool IsDeleted { get; set; } = false;  // soft delete
    public DateTime? DeletedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // RowVersion — star feature for concurrency
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    // Navigation
    public Patient Patient { get; set; } = null!;
    public Doctor Doctor { get; set; } = null!;
}