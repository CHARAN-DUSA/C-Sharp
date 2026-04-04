namespace DoctorBooking.API.Application.DTOs.Auth;

public class RegisterDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "Patient"; // Patient | Doctor | Admin
    public string? PhoneNumber { get; set; }
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    // Doctor-specific
    public string? Specialty { get; set; }
    public string? Qualifications { get; set; }
    public decimal? ConsultationFee { get; set; }
}