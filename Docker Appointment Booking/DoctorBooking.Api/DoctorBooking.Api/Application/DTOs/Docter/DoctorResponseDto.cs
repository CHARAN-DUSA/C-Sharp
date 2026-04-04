namespace DoctorBooking.API.Application.DTOs.Doctor;

public class DoctorResponseDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Specialty { get; set; } = string.Empty;
    public string? SubSpecialty { get; set; }
    public string Qualifications { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public string Bio { get; set; } = string.Empty;
    public decimal ConsultationFee { get; set; }
    public double Rating { get; set; }
    public int TotalReviews { get; set; }
    public string? ProfilePicture { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsVerified { get; set; }
    public string Languages { get; set; } = string.Empty;
    public string WorkingDays { get; set; } = string.Empty;
    public string? ClinicName { get; set; }
    public string? Address { get; set; }
}