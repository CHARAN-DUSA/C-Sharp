using DoctorBooking.API.Domain.Entities;

namespace DoctorBooking.API.Domain.Entities;

public class Patient
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string? BloodGroup { get; set; }
    public string? Allergies { get; set; }
    public string? MedicalHistory { get; set; }
    public string? EmergencyContact { get; set; }
    public string? Address { get; set; }
    public bool IsDeleted { get; set; } = false;

    // Navigation
    public AppUser User { get; set; } = null!;
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}