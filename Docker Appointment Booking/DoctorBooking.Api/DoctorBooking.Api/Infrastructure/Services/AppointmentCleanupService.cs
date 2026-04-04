using DoctorBooking.API.Application.Interfaces;
using DoctorBooking.API.Domain.Enums;
using DoctorBooking.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DoctorBooking.API.Infrastructure.Services;

public class AppointmentCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AppointmentCleanupService> _logger;

    public AppointmentCleanupService(
        IServiceScopeFactory scopeFactory,
        ILogger<AppointmentCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AppointmentCleanupService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunCleanupAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Cleanup cancelled.");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during appointment cleanup.");
            }

            try
            {
                // 🔥 Use shorter delay in development if needed
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Delay cancelled. Service shutting down.");
                break;
            }
        }

        _logger.LogInformation("AppointmentCleanupService stopped.");
    }

    private async Task RunCleanupAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var repo = scope.ServiceProvider.GetRequiredService<IAppointmentRepository>();
        var email = scope.ServiceProvider.GetRequiredService<EmailService>();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // ── 1. Mark No-Shows ───────────────────────────────
        var noShows = await repo.GetNoShowCandidatesAsync(stoppingToken);

        int noShowCount = 0;

        foreach (var appt in noShows)
        {
            if (stoppingToken.IsCancellationRequested)
                return;

            appt.Status = AppointmentStatus.NoShow;

            var updated = await repo.UpdateAsync(appt, stoppingToken);
            if (updated)
                noShowCount++;
        }

        // ── 2. Send Reminders ──────────────────────────────
        var tomorrow = DateTime.UtcNow.Date.AddDays(1);

        var reminders = await ctx.Appointments
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .Where(a =>
                a.AppointmentDate.Date == tomorrow &&
                a.Status == AppointmentStatus.Confirmed)
            .ToListAsync(stoppingToken);

        int reminderCount = 0;

        foreach (var appt in reminders)
        {
            if (stoppingToken.IsCancellationRequested)
                return;

            try
            {
                await email.SendAppointmentReminderAsync(
                    appt.Patient.User.Email!,
                    appt.Patient.User.FullName,
                    appt.Doctor.User.FullName,
                    appt.AppointmentDate,
                    appt.TimeSlot
                );

                reminderCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send reminder for AppointmentId: {Id}", appt.Id);
            }
        }

        // ── 3. Log Summary ────────────────────────────────
        _logger.LogInformation(
            "Cleanup completed: {NoShows} no-shows marked, {Reminders} reminders sent.",
            noShowCount,
            reminderCount
        );
    }
}