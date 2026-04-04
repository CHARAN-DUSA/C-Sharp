using FirebaseAdmin.Auth;

// ✅ Fixed
public class FirebaseAuthService
{
    public async Task<string?> VerifyToken(string idToken)
    {
        try
        {
            var decoded = await FirebaseAuth.DefaultInstance
                .VerifyIdTokenAsync(idToken);

            // phone_number lives in claims for phone auth
            if (decoded.Claims.TryGetValue("phone_number", out var phone))
                return phone?.ToString();

            // fallback: uid is always present
            return decoded.Uid;
        }
        catch (FirebaseAuthException ex)
        {
            Console.WriteLine($"Firebase token error: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected Firebase error: {ex.Message}");
            return null;
        }
    }
}