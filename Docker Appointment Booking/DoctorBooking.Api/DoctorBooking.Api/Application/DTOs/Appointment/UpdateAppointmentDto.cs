namespace DoctorBooking.API.Application.DTOs.Appointment;

public class UpdateAppointmentDto
{
    public string? Notes { get; set; }
    public string? Prescription { get; set; }
    public DateTime? FollowUpDate { get; set; }
    public string RowVersion { get; set; } = string.Empty; // required for concurrency check
}