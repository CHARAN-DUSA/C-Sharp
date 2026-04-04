// In Infrastructure/Data/DbSeeder.cs — ADD this file
using DoctorBooking.API.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DoctorBooking.API.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var ctx = services.GetRequiredService<AppDbContext>();

        // ── Seed Roles ─────────────────────────────────────────────────────────
        foreach (var role in new[] { "Patient", "Doctor", "Admin" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // ── Seed Demo Admin ─────────────────────────────────────────────────────
        if (await userManager.FindByEmailAsync("admin@demo.com") is null)
        {
            var admin = new AppUser
            {
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@demo.com",
                UserName = "admin@demo.com",
                Role = "Admin",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, "Demo@1234");
            await userManager.AddToRoleAsync(admin, "Admin");
        }

        // ── Seed Demo Patient ───────────────────────────────────────────────────
        if (await userManager.FindByEmailAsync("patient@demo.com") is null)
        {
            var patient = new AppUser
            {
                FirstName = "Demo",
                LastName = "Patient",
                Email = "patient@demo.com",
                UserName = "patient@demo.com",
                Role = "Patient",
                EmailConfirmed = true,
                PhoneNumber = "+919876543210"
            };
            await userManager.CreateAsync(patient, "Demo@1234");
            await userManager.AddToRoleAsync(patient, "Patient");
            ctx.Patients.Add(new Patient
            {
                UserId = patient.Id,
                Gender = "Male",
                DateOfBirth = new DateTime(1990, 1, 1)
            });
        }

        // ── Seed Demo Doctors ───────────────────────────────────────────────────
        var doctors = new[]
        {
            new { Email="dr.sharma@demo.com",  First="Rajesh",   Last="Sharma",   Spec="Cardiology",   Fee=800m,  Exp=12, Qual="MBBS, MD (Cardiology)", Bio="Senior cardiologist with 12 years of experience.", Phone="+919876543001" },
            new { Email="dr.priya@demo.com",   First="Priya",    Last="Patel",    Spec="Pediatrics",   Fee=600m,  Exp=8,  Qual="MBBS, DCH", Bio="Dedicated pediatrician caring for children.", Phone="+919876543002" },
            new { Email="dr.kumar@demo.com",   First="Arun",     Last="Kumar",    Spec="Neurology",    Fee=1000m, Exp=15, Qual="MBBS, DM (Neurology)", Bio="Expert neurologist specializing in brain disorders.", Phone="+919876543003" },
            new { Email="dr.mehta@demo.com",   First="Sunita",   Last="Mehta",    Spec="Dermatology",  Fee=500m,  Exp=6,  Qual="MBBS, MD (Dermatology)", Bio="Skin specialist for all dermatological conditions.", Phone="+919876543004" },
            new { Email="dr.verma@demo.com",   First="Vikram",   Last="Verma",    Spec="Orthopedics",  Fee=700m,  Exp=10, Qual="MBBS, MS (Ortho)", Bio="Orthopedic surgeon specializing in joint replacements.", Phone="+919876543005" },
            new { Email="dr.reddy@demo.com",   First="Lakshmi",  Last="Reddy",    Spec="Gynecology",   Fee=650m,  Exp=9,  Qual="MBBS, MS (OBG)", Bio="Women's health specialist with compassionate care.", Phone="+919876543006" },
        };

        foreach (var d in doctors)
        {
            if (await userManager.FindByEmailAsync(d.Email) is not null) continue;

            var user = new AppUser
            {
                FirstName = d.First,
                LastName = d.Last,
                Email = d.Email,
                UserName = d.Email,
                Role = "Doctor",
                EmailConfirmed = true,
                PhoneNumber = d.Phone
            };
            await userManager.CreateAsync(user, "Demo@1234");
            await userManager.AddToRoleAsync(user, "Doctor");

            ctx.Doctors.Add(new Doctor
            {
                UserId = user.Id,
                Specialty = d.Spec,
                Qualifications = d.Qual,
                ExperienceYears = d.Exp,
                ConsultationFee = d.Fee,
                Bio = d.Bio,
                Rating = Math.Round(3.5 + new Random().NextDouble() * 1.5, 1),
                TotalReviews = new Random().Next(10, 150),
                IsVerified = true,      // ← verified so they show in listing
                IsAvailable = true,
                Languages = "English, Hindi",
                WorkingDays = "Mon,Tue,Wed,Thu,Fri"
            });
        }

        await ctx.SaveChangesAsync();
    }
}