using System.Security.Cryptography;
using System.Text;

namespace DoctorBooking.API.Infrastructure.Services;

/// <summary>
/// Time-based One-Time Password (TOTP) service compatible with
/// Google Authenticator, Microsoft Authenticator, Authy.
/// </summary>
public class TwoFactorService
{
    private const int StepSeconds = 30;
    private const int CodeDigits = 6;

    // ── Generate a new secret key ──────────────────────────────
    public string GenerateSecret()
    {
        var key = new byte[20];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(key);
        return Base32Encode(key);
    }

    // ── Get QR Code URL for Google Authenticator ───────────────
    public string GetQrCodeUrl(string email, string secret, string issuer = "MediBook")
    {
        var encodedIssuer = Uri.EscapeDataString(issuer);
        var encodedEmail = Uri.EscapeDataString(email);
        var otpAuthUri = $"otpauth://totp/{encodedIssuer}:{encodedEmail}?secret={secret}&issuer={encodedIssuer}&digits={CodeDigits}&period={StepSeconds}";
        return $"https://api.qrserver.com/v1/create-qr-code/?data={Uri.EscapeDataString(otpAuthUri)}&size=200x200";
    }

    // ── Verify a TOTP code ─────────────────────────────────────
    public bool VerifyCode(string secret, string code)
    {
        if (string.IsNullOrWhiteSpace(code) || code.Length != CodeDigits) return false;
        if (!long.TryParse(code, out var inputCode)) return false;

        var secretBytes = Base32Decode(secret);
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // Allow ±1 step window (±30 seconds) to handle clock skew
        for (int i = -1; i <= 1; i++)
        {
            var step = (timestamp / StepSeconds) + i;
            var totp = GenerateTotp(secretBytes, step);
            if (totp == inputCode) return true;
        }

        return false;
    }

    // ── TOTP Algorithm (RFC 6238) ──────────────────────────────
    private static long GenerateTotp(byte[] secret, long step)
    {
        var stepBytes = BitConverter.GetBytes(step);
        if (BitConverter.IsLittleEndian) Array.Reverse(stepBytes);

        using var hmac = new HMACSHA1(secret);
        var hash = hmac.ComputeHash(stepBytes);
        var offset = hash[^1] & 0x0F;
        var binary = ((hash[offset] & 0x7F) << 24)
                   | ((hash[offset + 1] & 0xFF) << 16)
                   | ((hash[offset + 2] & 0xFF) << 8)
                   | (hash[offset + 3] & 0xFF);

        return binary % (long)Math.Pow(10, CodeDigits);
    }

    // ── Base32 helpers ─────────────────────────────────────────
    private static readonly char[] Base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".ToCharArray();

    private static string Base32Encode(byte[] data)
    {
        var sb = new StringBuilder();
        int buffer = data[0], bitsLeft = 8, i = 1;
        while (bitsLeft > 0 || i < data.Length)
        {
            if (bitsLeft < 5)
            {
                if (i < data.Length) { buffer <<= 8; buffer |= data[i++] & 0xFF; bitsLeft += 8; }
                else { buffer <<= 5 - bitsLeft; bitsLeft = 5; }
            }
            bitsLeft -= 5;
            sb.Append(Base32Chars[(buffer >> bitsLeft) & 0x1F]);
        }
        return sb.ToString();
    }

    private static byte[] Base32Decode(string base32)
    {
        base32 = base32.ToUpper().TrimEnd('=');
        var output = new List<byte>();
        int buffer = 0, bitsLeft = 0;
        foreach (var c in base32)
        {
            var index = Array.IndexOf(Base32Chars, c);
            if (index < 0) continue;
            buffer = (buffer << 5) | index;
            bitsLeft += 5;
            if (bitsLeft >= 8) { bitsLeft -= 8; output.Add((byte)(buffer >> bitsLeft)); }
        }
        return output.ToArray();
    }
}