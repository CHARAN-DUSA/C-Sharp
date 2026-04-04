namespace DoctorBooking.API.Application.DTOs.Auth;

public class GoogleLoginDto
{
    /// <summary>The ID token returned by Google Sign-In on the frontend.</summary>
    public string IdToken { get; set; } = string.Empty;

    /// <summary>Role to assign if this is a new registration via Google.</summary>
    public string Role { get; set; } = "Patient";
}