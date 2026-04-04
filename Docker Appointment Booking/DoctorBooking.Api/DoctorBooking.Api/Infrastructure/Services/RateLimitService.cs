//using Microsoft.Extensions.Caching.Memory;

//namespace DoctorBooking.API.Infrastructure.Services;

///// <summary>
///// Fine-grained per-phone-number rate limiter for OTP — sits on top of
///// the global IP-level rate limiting from AspNetCoreRateLimit.
///// </summary>
//public class OtpRateLimitService
//{
//    private readonly IMemoryCache _cache;
//    private readonly IConfiguration _config;

//    public OtpRateLimitService(IMemoryCache cache, IConfiguration config)
//    {
//        _cache = cache;
//        _config = config;
//    }

//    private int MaxAttempts =>
//     int.TryParse(_config["OtpSettings:MaxAttempts"], out var attempts)
//         ? attempts
//         : 3;

//    private int LockoutMins =>
//        int.TryParse(_config["OtpSettings:LockoutMinutes"], out var mins)
//            ? mins
//            : 5;

//    // ── Check if phone is rate-limited ─────────────────────────
//    public (bool IsLimited, int WaitMinutes) CheckSendLimit(string phoneNumber)
//    {
//        var key = $"otp_send:{phoneNumber}";
//        var data = _cache.Get<OtpRateData>(key);
//        if (data is null) return (false, 0);
//        if (data.LockedUntil.HasValue && data.LockedUntil > DateTime.UtcNow)
//        {
//            var wait = (int)(data.LockedUntil.Value - DateTime.UtcNow).TotalMinutes + 1;
//            return (true, wait);
//        }
//        return (false, 0);
//    }

//    // ── Record a send attempt ──────────────────────────────────
//    public void RecordSendAttempt(string phoneNumber)
//    {
//        var key = $"otp_send:{phoneNumber}";
//        var data = _cache.GetOrCreate(key, e =>
//        {
//            e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(LockoutMins + 5);
//            return new OtpRateData();
//        })!;

//        data.Attempts++;

//        if (data.Attempts >= MaxAttempts)
//        {
//            data.LockedUntil = DateTime.UtcNow.AddMinutes(LockoutMins);
//        }

//        _cache.Set(key, data, TimeSpan.FromMinutes(LockoutMins + 5));
    

//    // ── Clear after successful verification ───────────────────
//    public void ClearLimit(string phoneNumber)
    
//        _cache.Remove($"otp_send:{phoneNumber}");
//    }
//}

//public class OtpRateData
//{
//    public int Attempts { get; set; } = 0;
//    public DateTime? LockedUntil { get; set; }
//}