//using DoctorBooking.API.Domain.Entities;
//using DoctorBooking.API.Infrastructure.Data;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Org.BouncyCastle.Crypto.Generators;
//using Twilio;
//using Twilio.Rest.Api.V2010.Account;

//namespace DoctorBooking.API.Infrastructure.Services;

//public class OtpService
//{
//    private readonly AppDbContext _ctx;
//    private readonly UserManager<AppUser> _userManager;
//    private readonly IConfiguration _config;
//    private readonly ILogger<OtpService> _logger;

//    public OtpService(AppDbContext ctx, UserManager<AppUser> userManager,
//        IConfiguration config, ILogger<OtpService> logger)
//    {
//        _ctx = ctx;
//        _userManager = userManager;
//        _config = config;
//        _logger = logger;
//    }

//    // ── Send OTP via Twilio SMS ────────────────────────────────
//    public async Task<(bool Success, string Message)> SendOtpAsync(string phoneNumber)
//    {
//        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

//        // New users will have user = null here — that's fine; we still send OTP
//        if (user is not null)
//        {
//            // ── Lockout check ────────────────────────────────────
//            if (user.OtpLockedUntil.HasValue && user.OtpLockedUntil > DateTime.UtcNow)
//            {
//                var wait = (int)(user.OtpLockedUntil.Value - DateTime.UtcNow).TotalMinutes + 1;
//                return (false, $"Too many attempts. Try again in {wait} minute(s).");
//            }
//        }

//        // Generate a secure 6-digit OTP
//        var otp = GenerateOtp();
//        var expiry = int.Parse(_config["OtpSettings:ExpiryMinutes"]!);

//        if (user is not null)
//        {
//            user.OtpCode = BCrypt.Net.BCrypt.HashPassword(otp); // hash before storing
//            user.OtpExpiry = DateTime.UtcNow.AddMinutes(expiry);
//            user.OtpAttempts = 0;
//            user.OtpLockedUntil = null;
//            await _userManager.UpdateAsync(user);
//        }
//        else
//        {
//            // Store temp OTP in cache/DB for phone-number-based registration
//            // We store it directly on a placeholder until user registers
//            var tempKey = $"otp:{phoneNumber}";
//            _ctx.Database.ExecuteSqlRaw(
//                "INSERT INTO OtpCache (PhoneNumber, OtpHash, Expiry) VALUES ({0},{1},{2}) " +
//                "ON CONFLICT(PhoneNumber) DO UPDATE SET OtpHash=excluded.OtpHash, Expiry=excluded.Expiry",
//                phoneNumber, BCrypt.Net.BCrypt.HashPassword(otp), DateTime.UtcNow.AddMinutes(expiry));
//        }

//        // ── Send via Twilio ────────────────────────────────────
//        await SendSmsAsync(phoneNumber, $"Your MediBook OTP is: {otp}. Valid for {expiry} minutes. Do not share this code.");

//        _logger.LogInformation("OTP sent to {Phone}", phoneNumber);
//        return (true, "OTP sent successfully.");
//    }

//    // ── Verify OTP for existing users ─────────────────────────
//    public async Task<(bool Valid, string Message)> VerifyOtpForUserAsync(AppUser user, string otpCode)
//    {
//        var maxAttempts = int.Parse(_config["OtpSettings:MaxAttempts"]!);
//        var lockMins = int.Parse(_config["OtpSettings:LockoutMinutes"]!);

//        // Already locked?
//        if (user.OtpLockedUntil.HasValue && user.OtpLockedUntil > DateTime.UtcNow)
//        {
//            var wait = (int)(user.OtpLockedUntil.Value - DateTime.UtcNow).TotalMinutes + 1;
//            return (false, $"Account locked. Try again in {wait} minute(s).");
//        }

//        // Expired?
//        if (user.OtpExpiry is null || user.OtpExpiry < DateTime.UtcNow)
//        {
//            return (false, "OTP has expired. Please request a new one.");
//        }

//        // Verify hash
//        bool valid = BCrypt.Net.BCrypt.Verify(otpCode, user.OtpCode);

//        if (!valid)
//        {
//            user.OtpAttempts++;

//            if (user.OtpAttempts >= maxAttempts)
//            {
//                user.OtpLockedUntil = DateTime.UtcNow.AddMinutes(lockMins);
//                user.OtpCode = null;
//                await _userManager.UpdateAsync(user);
//                return (false, $"Too many failed attempts. Account locked for {lockMins} minutes.");
//            }

//            await _userManager.UpdateAsync(user);
//            var remaining = maxAttempts - user.OtpAttempts;
//            return (false, $"Invalid OTP. {remaining} attempt(s) remaining.");
//        }

//        // Clear OTP after successful verification
//        user.OtpCode = null;
//        user.OtpExpiry = null;
//        user.OtpAttempts = 0;
//        user.OtpLockedUntil = null;
//        await _userManager.UpdateAsync(user);

//        return (true, "OTP verified successfully.");
//    }

//    // ── Helpers ────────────────────────────────────────────────
//    private static string GenerateOtp()
//    {
//        var random = new Random();
//        return random.Next(100000, 999999).ToString();
//    }

//    private async Task SendSmsAsync(string to, string body)
//    {
//        var sid = _config["TwilioSettings:AccountSid"]!;
//        var token = _config["TwilioSettings:AuthToken"]!;
//        var from = _config["TwilioSettings:FromNumber"]!;

//        TwilioClient.Init(sid, token);
//        try
//        {
//            await MessageResource.CreateAsync(
//                body: body,
//                from: new Twilio.Types.PhoneNumber(from),
//                to: new Twilio.Types.PhoneNumber(to));
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Failed to send SMS to {Phone}", to);
//            // In development, log the OTP instead of throwing
//            _logger.LogWarning("DEV MODE OTP for {Phone}: {Body}", to, body);
//        }
//    }
//}
