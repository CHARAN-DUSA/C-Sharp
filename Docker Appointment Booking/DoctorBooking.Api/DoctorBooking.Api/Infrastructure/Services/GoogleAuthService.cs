using Google.Apis.Auth;
using Microsoft.SqlServer.Server;

namespace DoctorBooking.API.Infrastructure.Services;

public class GoogleAuthService
{
    private readonly IConfiguration _config;
    private readonly ILogger<GoogleAuthService> _logger;

    public GoogleAuthService(IConfiguration config, ILogger<GoogleAuthService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleTokenAsync(string idToken)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _config["GoogleAuth:ClientId"]! }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            return payload;
        }
        catch (InvalidJwtException ex)
        {
            _logger.LogWarning("Invalid Google token: {Message}", ex.Message);
            return null;
        }
    }
}