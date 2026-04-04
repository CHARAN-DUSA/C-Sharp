using Microsoft.AspNetCore.Identity;

namespace DoctorBooking.API.Domain.Entities;

public class AppUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = "Patient";
    public string? ProfilePicture { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // OAuth
    public string? GoogleId { get; set; }
    public string? FirebaseUid { get; set; }
    public string AuthProvider { get; set; } = "Local"; // Local | Google | Firebase

    // 2FA (TOTP - Google Authenticator)
    public bool TwoFactorEnabled { get; set; } = false;
    public string? TwoFactorSecret { get; set; }

    // Navigation
    public Doctor? Doctor { get; set; }
    public Patient? Patient { get; set; }

    public string FullName => $"{FirstName} {LastName}".Trim();
}