namespace DoctorBooking.API.Application.DTOs.Chat;

public class SendMessageDto
{
    public string ReceiverId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int? AppointmentId { get; set; }
}