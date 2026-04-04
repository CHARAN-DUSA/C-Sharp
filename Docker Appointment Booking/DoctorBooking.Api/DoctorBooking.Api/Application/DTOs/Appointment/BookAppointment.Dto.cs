namespace DoctorBooking.API.Application.DTOs.Appointment;

public class BookAppointmentDto
{
    public int DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string TimeSlot { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}