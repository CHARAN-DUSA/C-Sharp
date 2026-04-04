namespace DoctorBooking.API.Domain.Entities;

public class Notification
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "System"; // Appointment, Message, Reminder, System
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? ReferenceId { get; set; }
    public bool IsDeleted { get; set; } = false;

    // Navigation
    public AppUser User { get; set; } = null!;
}