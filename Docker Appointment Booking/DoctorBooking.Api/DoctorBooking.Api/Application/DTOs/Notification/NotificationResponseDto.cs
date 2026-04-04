namespace DoctorBooking.API.Application.DTOs.Notification
{
    public record NotificationResponseDto(
    int Id,
    string Title,
    string Body,
    bool IsRead,
    DateTime CreatedAt
);
}
