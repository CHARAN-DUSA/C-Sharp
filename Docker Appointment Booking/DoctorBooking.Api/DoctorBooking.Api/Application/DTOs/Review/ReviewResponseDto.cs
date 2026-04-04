namespace DoctorBooking.API.Application.DTOs.Review;

public record ReviewResponseDto(
    int Id,
    string PatientName,
    int Rating,
    string? Comment,
    DateTime CreatedAt
);