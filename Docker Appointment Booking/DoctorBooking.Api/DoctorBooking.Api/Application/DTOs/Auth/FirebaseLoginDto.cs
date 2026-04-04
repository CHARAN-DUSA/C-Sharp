namespace DoctorBooking.API.Application.DTOs.Auth;

/// <summary>
/// Sent from frontend after Firebase verifies OTP (phone/email) or Google sign-in.
/// The frontend gets an ID token from Firebase and sends it here for backend verification.
/// </summary>
public class FirebaseLoginDto
{
    /// <summary>Firebase ID token from client SDK after successful auth.</summary>
    public string IdToken { get; set; } = string.Empty;

    /// <summary>Role to assign if this is a first-time registration.</summary>
    public string Role { get; set; } = "Patient";

    /// <summary>Display name (used only when registering a new account).</summary>
    public string? DisplayName { get; set; }
}