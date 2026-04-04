namespace DoctorBooking.API.Application.DTOs.TimeSlot;

public class TimeSlotDto
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsAvailable { get; set; }
}

public class CreateTimeSlotDto
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}