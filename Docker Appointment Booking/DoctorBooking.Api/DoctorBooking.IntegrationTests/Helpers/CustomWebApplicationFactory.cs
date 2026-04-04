using DoctorBooking.API.Domain.Entities;
using DoctorBooking.API.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
namespace DoctorBooking.IntegrationTests.Helpers;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Replace real DB with in-memory
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor is not null) services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid()));

            // Build and seed
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var um = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var rm = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            ctx.Database.EnsureCreated();
            SeedTestData(ctx, um, rm).GetAwaiter().GetResult();
        });
    }

    public static async Task SeedTestData(AppDbContext ctx,
        UserManager<AppUser> um, RoleManager<IdentityRole> rm)
    {
        foreach (var role in new[] { "Patient", "Doctor", "Admin" })
            if (!await rm.RoleExistsAsync(role))
                await rm.CreateAsync(new IdentityRole(role));

        // Seed patient
        var patient = new AppUser
        {
            UserName = "patient@test.com",
            Email = "patient@test.com",
            FirstName = "Test",
            LastName = "Patient",
            Role = "Patient"
        };
        await um.CreateAsync(patient, "Test@1234");
        await um.AddToRoleAsync(patient, "Patient");
        ctx.Patients.Add(new Patient
        {
            UserId = patient.Id,
            Gender = "Male",
            DateOfBirth = DateTime.UtcNow.AddYears(-30)
        });

        // Seed doctor
        var doctor = new AppUser
        {
            UserName = "doctor@test.com",
            Email = "doctor@test.com",
            FirstName = "Test",
            LastName = "Doctor",
            Role = "Doctor"
        };
        await um.CreateAsync(doctor, "Test@1234");
        await um.AddToRoleAsync(doctor, "Doctor");
        ctx.Doctors.Add(new Doctor
        {
            UserId = doctor.Id,
            Specialty = "General",
            Qualifications = "MBBS",
            ConsultationFee = 500,
            IsVerified = true
        });

        await ctx.SaveChangesAsync();
    }
}