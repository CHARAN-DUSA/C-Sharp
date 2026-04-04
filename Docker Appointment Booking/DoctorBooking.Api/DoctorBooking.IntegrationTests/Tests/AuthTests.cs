using DoctorBooking.IntegrationTests.Helpers;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace DoctorBooking.IntegrationTests.Tests;

public class AuthTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthTests(CustomWebApplicationFactory factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task Register_WithValidData_Returns200()
    {
        var dto = new
        {
            firstName = "John",
            lastName = "Doe",
            email = $"john{Guid.NewGuid()}@test.com",
            password = "Test@1234",
            role = "Patient",
            gender = "Male"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<dynamic>();
        ((string)body!.token).Should().NotBeNullOrEmpty();
        ((string)body.role).Should().Be("Patient");
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var dto = new { email = "patient@test.com", password = "Test@1234" };

        var response = await _client.PostAsJsonAsync("/api/auth/login", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<dynamic>();
        ((string)body!.token).Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401()
    {
        var dto = new { email = "patient@test.com", password = "WrongPass!" };

        var response = await _client.PostAsJsonAsync("/api/auth/login", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_Returns409()
    {
        var dto = new
        {
            firstName = "Jane",
            lastName = "Doe",
            email = "patient@test.com", // already exists
            password = "Test@1234",
            role = "Patient"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Register_WithInvalidRole_Returns400()
    {
        var dto = new
        {
            firstName = "Bad",
            lastName = "User",
            email = "bad@test.com",
            password = "Test@1234",
            role = "SuperAdmin" // invalid
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}