using DoctorBooking.API.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DoctorBooking.API.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<TimeSlot> TimeSlots { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ── Soft delete global query filters ─────────────────────
        builder.Entity<Doctor>().HasQueryFilter(d => !d.IsDeleted);
        builder.Entity<Patient>().HasQueryFilter(p => !p.IsDeleted);
        builder.Entity<Appointment>().HasQueryFilter(a => !a.IsDeleted);
        builder.Entity<ChatMessage>().HasQueryFilter(c => !c.IsDeleted);
        builder.Entity<Notification>().HasQueryFilter(n => !n.IsDeleted);
        builder.Entity<Review>().HasQueryFilter(r => !r.IsDeleted);
        builder.Entity<AppUser>().HasQueryFilter(u => !u.IsDeleted);
        builder.Entity<TimeSlot>().HasQueryFilter(t => !t.Doctor.IsDeleted);

        // ── RowVersion (optimistic concurrency) ──────────────────
        builder.Entity<Appointment>()
            .Property(a => a.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.Entity<Doctor>()
            .Property(d => d.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        // ── Appointments ─────────────────────────────────────────
        builder.Entity<Appointment>(entity =>
        {
            entity.HasOne(a => a.Patient)
                  .WithMany(p => p.Appointments)
                  .HasForeignKey(a => a.PatientId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.Doctor)
                  .WithMany(d => d.Appointments)
                  .HasForeignKey(a => a.DoctorId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.Property(a => a.ConsultationFee).HasPrecision(18, 2);
        });

        // ── Doctor ───────────────────────────────────────────────
        builder.Entity<Doctor>(entity =>
        {
            entity.HasOne(d => d.User)
                  .WithOne(u => u.Doctor)
                  .HasForeignKey<Doctor>(d => d.UserId);

            entity.Property(d => d.ConsultationFee).HasPrecision(18, 2);
            entity.Property(d => d.Rating).HasPrecision(3, 2);
        });

        // ── Patient ──────────────────────────────────────────────
        builder.Entity<Patient>()
            .HasOne(p => p.User)
            .WithOne(u => u.Patient)
            .HasForeignKey<Patient>(p => p.UserId);

        // ── TimeSlot ─────────────────────────────────────────────
        builder.Entity<TimeSlot>()
            .HasOne(t => t.Doctor)
            .WithMany(d => d.TimeSlots)
            .HasForeignKey(t => t.DoctorId);

        // ── Chat ─────────────────────────────────────────────────
        builder.Entity<ChatMessage>(entity =>
        {
            entity.HasOne(c => c.Sender)
                  .WithMany()
                  .HasForeignKey(c => c.SenderId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.Receiver)
                  .WithMany()
                  .HasForeignKey(c => c.ReceiverId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ── Notifications ────────────────────────────────────────
        builder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId);

        // ── Reviews ──────────────────────────────────────────────
        builder.Entity<Review>(entity =>
        {
            entity.HasOne(r => r.Doctor)
                  .WithMany(d => d.Reviews)
                  .HasForeignKey(r => r.DoctorId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.Patient)
                  .WithMany()
                  .HasForeignKey(r => r.PatientId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ── Indexes ──────────────────────────────────────────────
        builder.Entity<Appointment>()
            .HasIndex(a => new { a.DoctorId, a.AppointmentDate, a.TimeSlot })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}