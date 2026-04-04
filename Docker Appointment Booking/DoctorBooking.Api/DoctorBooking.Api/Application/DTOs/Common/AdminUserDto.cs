using DoctorBooking.API.Domain.Enums;

namespace DoctorBooking.API.Application.DTOs.Common;

public record AdminUserDto(
    string Id,
    string FullName,
    string Email,
    string Role,
    bool IsDeleted,
    DateTime CreatedAt
);