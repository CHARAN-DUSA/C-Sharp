namespace DoctorBooking.API.Application.DTOs.Doctor;

public class DoctorAvailabilityDto
{
    /// <summary>Specific date for which to generate slots (single day).</summary>
    public DateTime Date { get; set; } = DateTime.UtcNow.Date;
    public string StartTime { get; set; } = "09:00";
    public string EndTime { get; set; } = "17:00";
    public int SlotDurationMinutes { get; set; } = 30;
}

public class UpdateSlotDto
{
    public int SlotId { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
}