using DoctorBooking.API.Domain.Entities;
using System.ComponentModel.DataAnnotations;

public class Doctor
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public AppUser User { get; set; } = null!;
    public string Bio { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public decimal ConsultationFee { get; set; }
    public bool IsDeleted { get; set; } = false;
    public bool IsApproved { get; set; } = false;
    public bool IsVerified { get; set; } = false;

    public string Specialty { get; set; } = string.Empty;        // ✅ FIX
    public string Qualifications { get; set; } = string.Empty;
    public string Languages { get; set; } = string.Empty;   // ✅ FIX
    public string? ClinicName { get; set; }                 // ✅ FIX
    public string? Address { get; set; }
    public double Rating { get; set; } = 0;

    public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public int TotalReviews { get; set; } = 0;
    public string? WorkingDays { get; set; }
    public int? ExperienceYears { get; set; }
    public bool IsAvailable { get; set; } = false;
    [Timestamp]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}