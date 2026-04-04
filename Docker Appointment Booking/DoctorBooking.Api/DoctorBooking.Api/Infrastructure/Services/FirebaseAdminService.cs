using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

namespace DoctorBooking.API.Infrastructure.Services;

/// <summary>
/// Verifies Firebase ID tokens server-side using Firebase Admin SDK.
/// Replaces Twilio OTP — Firebase handles phone/email OTP on the frontend,
/// and this service verifies the resulting token on the backend.
/// </summary>
public class FirebaseAdminService
{
    private readonly ILogger<FirebaseAdminService> _logger;

    public FirebaseAdminService(IConfiguration config, ILogger<FirebaseAdminService> logger)
    {
        _logger = logger;

        // Initialize Firebase Admin SDK once
        if (FirebaseApp.DefaultInstance == null)
        {
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromJson($$"""
                {
                  "type": "service_account",
                  "project_id": "{{config["Firebase:ProjectId"]}}",
                  "private_key_id": "not-needed-for-token-verify",
                  "private_key": "not-needed-for-token-verify",
                  "client_email": "not-needed-for-token-verify",
                  "client_id": "not-needed-for-token-verify",
                  "auth_uri": "https://accounts.google.com/o/oauth2/auth",
                  "token_uri": "https://oauth2.googleapis.com/token"
                }
                """),
                ProjectId = config["Firebase:ProjectId"]
            });
        }
    }

    /// <summary>
    /// Verifies a Firebase ID token (from frontend after OTP/Google login).
    /// Returns the decoded token if valid, null if invalid.
    /// </summary>
    public async Task<FirebaseToken?> VerifyIdTokenAsync(string idToken)
    {
        try
        {
            var decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
            return decoded;
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogWarning("Firebase token verification failed: {Message}", ex.Message);
            return null;
        }
    }
}