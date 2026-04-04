using DoctorBooking.API.Domain.Enums;

namespace DoctorBooking.API.Application.DTOs.Appointment;

public class AppointmentResponseDto
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public string DoctorSpecialty { get; set; } = string.Empty;
    public string? DoctorProfilePicture { get; set; }
    public string? PatientProfilePicture { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string TimeSlot { get; set; } = string.Empty;
    public AppointmentStatus Status { get; set; }
    public string StatusText => Status.ToString();
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string? Prescription { get; set; }
    public decimal ConsultationFee { get; set; }
    public string RowVersion { get; set; } = string.Empty; // base64 for concurrency
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }
}