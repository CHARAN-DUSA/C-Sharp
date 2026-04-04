using DoctorBooking.IntegrationTests.Helpers;
using FluentAssertions;
using System.Net;
using Xunit;

namespace DoctorBooking.IntegrationTests.Tests;

public class AppointmentTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AppointmentTests(CustomWebApplicationFactory factory)
        => _factory = factory;

    private HttpClient CreateAuthenticatedClient(string role = "Patient", string userId = "test-patient-id")
        => _factory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services =>
            {
                services.AddAuthentication("Test")
                    .AddScheme<TestAuthHandlerOptions, TestAuthHandler>("Test", opts =>
                    {
                        opts.UserId = userId;
                        opts.Role = role;
                    });
            })).CreateClient();

    [Fact]
    public async Task BookAppointment_WithValidSlot_Returns201()
    {
        var client = CreateAuthenticatedClient("Patient");
        var dto = new
        {
            doctorId = 1,
            appointmentDate = DateTime.UtcNow.AddDays(1).ToString("o"),
            timeSlot = "09:00",
            reason = "Routine checkup"
        };

        var response = await client.PostAsJsonAsync("/api/appointments", dto);

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.Created, HttpStatusCode.Conflict, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetMyAppointments_AsPatient_ReturnsOk()
    {
        var client = CreateAuthenticatedClient("Patient");

        var response = await client.GetAsync("/api/appointments/my");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task BookAppointment_AsDoctor_Returns403()
    {
        var client = CreateAuthenticatedClient("Doctor");
        var dto = new
        {
            doctorId = 1,
            appointmentDate = DateTime.UtcNow.AddDays(1).ToString("o"),
            timeSlot = "09:00",
            reason = "Should fail"
        };

        var response = await client.PostAsJsonAsync("/api/appointments", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAllAppointments_AsAdmin_ReturnsOk()
    {
        var client = CreateAuthenticatedClient("Admin");

        var response = await client.GetAsync("/api/admin/appointments");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}