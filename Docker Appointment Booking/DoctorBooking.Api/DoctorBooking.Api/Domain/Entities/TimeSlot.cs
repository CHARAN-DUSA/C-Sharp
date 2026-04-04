namespace DoctorBooking.API.Domain.Entities;

public class TimeSlot
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public DateTime Date { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public bool IsBooked { get; set; } = false;
    public bool IsBlocked { get; set; } = false;
    public bool IsDeleted { get; set; } = false;

    // Navigation
    public Doctor Doctor { get; set; } = null!;
}