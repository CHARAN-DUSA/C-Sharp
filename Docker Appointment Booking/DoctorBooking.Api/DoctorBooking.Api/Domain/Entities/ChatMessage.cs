namespace DoctorBooking.API.Domain.Entities;

public class ChatMessage
{
    public int Id { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public string ReceiverId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;
    public int? AppointmentId { get; set; }
    public bool IsDeleted { get; set; } = false;

    // Navigation
    public AppUser Sender { get; set; } = null!;
    public AppUser Receiver { get; set; } = null!;
}