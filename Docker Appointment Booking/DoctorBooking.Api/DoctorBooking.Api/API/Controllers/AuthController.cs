using DoctorBooking.API.Application.DTOs.Auth;
using DoctorBooking.API.Domain.Entities;
using DoctorBooking.API.Domain.Enums;
using DoctorBooking.API.Infrastructure.Data;
using DoctorBooking.API.Infrastructure.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DoctorBooking.API.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly TokenService _tokenService;
    private readonly EmailService _emailService;
    private readonly GoogleAuthService _googleAuth;
    private readonly FirebaseAdminService _firebase;
    private readonly TwoFactorService _twoFactor;
    private readonly AppDbContext _ctx;

    // In-memory OTP store (for password reset only)
    // In production replace with IDistributedCache / Redis
    private static readonly Dictionary<string, (string Otp, DateTime Expiry, int Attempts)> _otpStore = new();
    private static readonly object _otpLock = new();

    public AuthController(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        TokenService tokenService,
        EmailService emailService,
        GoogleAuthService googleAuth,
        FirebaseAdminService firebase,
        TwoFactorService twoFactor,
        AppDbContext ctx)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _emailService = emailService;
        _googleAuth = googleAuth;
        _firebase = firebase;
        _twoFactor = twoFactor;
        _ctx = ctx;
    }

    // ══════════════════════════════════════════════════════════
    // 1. Register (email + password)
    // ══════════════════════════════════════════════════════════
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
    {
        if (await _userManager.FindByEmailAsync(dto.Email) is not null)
            return Conflict(new { detail = "An account with this email already exists." });

        var user = new AppUser
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            UserName = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Role = dto.Role,
            AuthProvider = "Local"
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(new { detail = string.Join("; ", result.Errors.Select(e => e.Description)) });

        await _userManager.AddToRoleAsync(user, dto.Role);
        await CreateRoleProfileAsync(user, dto);

        // Send welcome email
        _ = Task.Run(() => _emailService.SendWelcomeEmailAsync(
            user.Email!, user.FullName, user.Role));

        return Ok(BuildAuthResponse(user));
    }

    // ══════════════════════════════════════════════════════════
    // 2. Login (email + password)
    // ══════════════════════════════════════════════════════════
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null || user.IsDeleted)
            return Unauthorized(new { detail = "Invalid email or password." });

        var result = await _signInManager.CheckPasswordSignInAsync(
            user, dto.Password, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
                return StatusCode(429, new { detail = "Account locked. Too many failed attempts. Try again in 5 minutes." });
            return Unauthorized(new { detail = "Invalid email or password." });
        }

        if (user.TwoFactorEnabled)
            return Ok(new { requiresTwoFactor = true, userId = user.Id });

        return Ok(BuildAuthResponse(user));
    }

    // ══════════════════════════════════════════════════════════
    // 3. Google OAuth (server-side token verification)
    // ══════════════════════════════════════════════════════════
    [HttpPost("google")]
    public async Task<ActionResult<AuthResponseDto>> GoogleLogin([FromBody] GoogleLoginDto dto)
    {
        var payload = await _googleAuth.VerifyGoogleTokenAsync(dto.IdToken);
        if (payload is null)
            return Unauthorized(new { detail = "Invalid Google token. Please sign in again." });

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.GoogleId == payload.Subject)
                ?? await _userManager.FindByEmailAsync(payload.Email);

        if (user is null)
        {
            var nameParts = (payload.Name ?? payload.Email).Split(' ', 2);
            user = new AppUser
            {
                FirstName = nameParts[0],
                LastName = nameParts.Length > 1 ? nameParts[1] : "",
                Email = payload.Email,
                UserName = payload.Email,
                GoogleId = payload.Subject,
                AuthProvider = "Google",
                Role = dto.Role,
                EmailConfirmed = true,
                ProfilePicture = payload.Picture
            };
            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
                return BadRequest(new { detail = string.Join("; ", result.Errors.Select(e => e.Description)) });

            await _userManager.AddToRoleAsync(user, dto.Role);
            await CreateRoleProfileAsync(user, new RegisterDto { Role = dto.Role });

            _ = Task.Run(() => _emailService.SendWelcomeEmailAsync(
                user.Email!, user.FullName, user.Role));
        }
        else
        {
            if (user.IsDeleted)
                return Unauthorized(new { detail = "This account has been disabled." });
            if (string.IsNullOrEmpty(user.GoogleId))
            {
                user.GoogleId = payload.Subject;
                user.AuthProvider = "Google";
                await _userManager.UpdateAsync(user);
            }
        }

        if (user.TwoFactorEnabled)
            return Ok(new { requiresTwoFactor = true, userId = user.Id });

        return Ok(BuildAuthResponse(user));
    }

    // ══════════════════════════════════════════════════════════
    // 4. Firebase Login (phone OTP + email OTP via Firebase)
    //    Frontend verifies OTP with Firebase → gets ID token → sends here
    // ══════════════════════════════════════════════════════════
    [HttpPost("firebase-login")]
    public async Task<ActionResult<AuthResponseDto>> FirebaseLogin([FromBody] FirebaseLoginDto dto)
    {
        var decoded = await _firebase.VerifyIdTokenAsync(dto.IdToken);
        if (decoded is null)
            return Unauthorized(new { detail = "Invalid Firebase token." });

        var firebaseUid = decoded.Uid;
        var email = decoded.Claims.GetValueOrDefault("email")?.ToString();
        var phone = decoded.Claims.GetValueOrDefault("phone_number")?.ToString();
        var name = decoded.Claims.GetValueOrDefault("name")?.ToString()
                       ?? dto.DisplayName
                       ?? "MediBook User";

        // Find existing user by Firebase UID, email, or phone
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid)
                ?? (email != null ? await _userManager.FindByEmailAsync(email) : null)
                ?? (phone != null ? await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phone) : null);

        if (user is null)
        {
            // Auto-register on first Firebase login
            var nameParts = name.Split(' ', 2);
            user = new AppUser
            {
                FirstName = nameParts[0],
                LastName = nameParts.Length > 1 ? nameParts[1] : "",
                Email = email ?? $"{firebaseUid}@firebase.medibook.com",
                UserName = email ?? firebaseUid,
                PhoneNumber = phone,
                FirebaseUid = firebaseUid,
                AuthProvider = "Firebase",
                Role = dto.Role,
                EmailConfirmed = email != null,
                PhoneNumberConfirmed = phone != null
            };
            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
                return BadRequest(new { detail = string.Join("; ", result.Errors.Select(e => e.Description)) });

            await _userManager.AddToRoleAsync(user, dto.Role);
            await CreateRoleProfileAsync(user, new RegisterDto { Role = dto.Role });

            if (email != null)
                _ = Task.Run(() => _emailService.SendWelcomeEmailAsync(
                    email, user.FullName, user.Role));
        }
        else
        {
            if (user.IsDeleted)
                return Unauthorized(new { detail = "This account has been disabled." });
            if (string.IsNullOrEmpty(user.FirebaseUid))
            {
                user.FirebaseUid = firebaseUid;
                user.AuthProvider = "Firebase";
                await _userManager.UpdateAsync(user);
            }
        }

        if (user.TwoFactorEnabled)
            return Ok(new { requiresTwoFactor = true, userId = user.Id });

        return Ok(BuildAuthResponse(user));
    }

    // ══════════════════════════════════════════════════════════
    // 5. Forgot Password — send OTP to email
    // ══════════════════════════════════════════════════════════
    [HttpPost("forgot-password/send-otp")]
    public async Task<IActionResult> SendForgotPasswordOtp([FromBody] ForgotPasswordOtpRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
            return BadRequest(new { message = "Email is required" });

        // Normalize email (IMPORTANT FIX)
        var email = dto.Email.Trim().ToLower();

        Console.WriteLine("🔍 Forgot password request for: " + email);

        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            Console.WriteLine("❌ User NOT FOUND");
            return BadRequest(new { message = "User not found with this email" });
        }

        Console.WriteLine("✅ User FOUND: " + user.Email);

        // Generate OTP
        var otp = new Random().Next(100000, 999999).ToString();

        lock (_otpLock)
        {
            _otpStore[email] = (otp, DateTime.UtcNow.AddMinutes(10), 0);
        }

        try
        {
            await _emailService.SendPasswordResetOtpAsync(user.Email!, user.FullName ?? "User", otp);
            Console.WriteLine("📧 OTP email SENT to: " + user.Email);
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ EMAIL ERROR: " + ex.Message);
            return StatusCode(500, new { message = "Failed to send OTP email" });
        }

        return Ok(new { message = "OTP sent to your email. Valid for 10 minutes." });
    }

    // ══════════════════════════════════════════════════════════
    // 6. Forgot Password — verify OTP + set new password
    // ══════════════════════════════════════════════════════════
    [HttpPost("forgot-password/verify-otp")]
    public async Task<IActionResult> VerifyForgotPasswordOtp([FromBody] VerifyForgotPasswordOtpDto dto)
    {
        var email = dto.Email.Trim().ToLower();

        Console.WriteLine("🔐 OTP VERIFY for: " + email);

        (string Otp, DateTime Expiry, int Attempts) stored;

        lock (_otpLock)
        {
            if (!_otpStore.TryGetValue(email, out stored))
                return BadRequest(new { detail = "No OTP request found. Please request a new OTP." });
        }

        if (stored.Expiry < DateTime.UtcNow)
        {
            lock (_otpLock) { _otpStore.Remove(email); }
            return BadRequest(new { detail = "OTP has expired. Please request a new one." });
        }

        if (stored.Attempts >= 3)
        {
            lock (_otpLock) { _otpStore.Remove(email); }
            return StatusCode(429, new { detail = "Too many failed attempts. Please request a new OTP." });
        }

        if (stored.Otp != dto.Otp)
        {
            lock (_otpLock)
            {
                _otpStore[email] = (stored.Otp, stored.Expiry, stored.Attempts + 1);
            }
            return BadRequest(new { detail = "Invalid OTP" });
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return BadRequest(new { detail = "User not found" });

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

        if (!result.Succeeded)
            return BadRequest(new { detail = string.Join("; ", result.Errors.Select(e => e.Description)) });

        lock (_otpLock) { _otpStore.Remove(email); }

        Console.WriteLine("✅ Password reset SUCCESS for: " + email);

        return Ok(new { message = "Password reset successfully. You can now log in." });
    }

    // ══════════════════════════════════════════════════════════
    // 7. Two-Factor: Setup
    // ══════════════════════════════════════════════════════════
    [HttpPost("2fa/setup")]
    [Authorize]
    public async Task<ActionResult<TwoFactorSetupResponseDto>> SetupTwoFactor()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();

        var secret = _twoFactor.GenerateSecret();
        user.TwoFactorSecret = secret;
        await _userManager.UpdateAsync(user);

        return Ok(new TwoFactorSetupResponseDto
        {
            Secret = secret,
            QrCodeImageUrl = _twoFactor.GetQrCodeUrl(user.Email!, secret),
            ManualEntryKey = secret
        });
    }

    // ══════════════════════════════════════════════════════════
    // 8. Two-Factor: Enable
    // ══════════════════════════════════════════════════════════
    [HttpPost("2fa/enable")]
    [Authorize]
    public async Task<IActionResult> EnableTwoFactor([FromBody] TwoFactorVerifyDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();

        if (string.IsNullOrEmpty(user.TwoFactorSecret))
            return BadRequest(new { detail = "Call /2fa/setup first." });

        if (!_twoFactor.VerifyCode(user.TwoFactorSecret, dto.Code))
            return Unauthorized(new { detail = "Invalid code." });

        user.TwoFactorEnabled = true;
        await _userManager.UpdateAsync(user);
        return Ok(new { message = "2FA enabled successfully." });
    }

    // ══════════════════════════════════════════════════════════
    // 9. Two-Factor: Disable
    // ══════════════════════════════════════════════════════════
    [HttpPost("2fa/disable")]
    [Authorize]
    public async Task<IActionResult> DisableTwoFactor([FromBody] TwoFactorVerifyDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();

        if (!_twoFactor.VerifyCode(user.TwoFactorSecret!, dto.Code))
            return Unauthorized(new { detail = "Invalid code." });

        user.TwoFactorEnabled = false;
        user.TwoFactorSecret = null;
        await _userManager.UpdateAsync(user);
        return Ok(new { message = "2FA disabled." });
    }

    // ══════════════════════════════════════════════════════════
    // 10. Two-Factor: Verify login
    // ══════════════════════════════════════════════════════════
    [HttpPost("2fa/verify")]
    public async Task<ActionResult<AuthResponseDto>> VerifyTwoFactor([FromBody] TwoFactorLoginDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);
        if (user is null || !user.TwoFactorEnabled)
            return BadRequest(new { detail = "Invalid request." });

        if (!_twoFactor.VerifyCode(user.TwoFactorSecret!, dto.Code))
            return Unauthorized(new { detail = "Invalid authenticator code." });

        return Ok(BuildAuthResponse(user));
    }

    // ── PHONE + PASSWORD LOGIN ─────────────────────────────────
    [HttpPost("login-phone")]
    public async Task<IActionResult> LoginWithPhone([FromBody] PhoneLoginDto dto)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber);

        if (user is null || user.IsDeleted)
            return Unauthorized(new { detail = "Invalid phone number or password." });

        var result = await _signInManager.CheckPasswordSignInAsync(
            user, dto.Password, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
                return StatusCode(429, new { detail = "Account locked. Try again in 5 minutes." });
            return Unauthorized(new { detail = "Invalid phone number or password." });
        }

        if (user.TwoFactorEnabled)
            return Ok(new { requiresTwoFactor = true, userId = user.Id });

        return Ok(BuildAuthResponse(user));
    }

    // ══════════════════════════════════════════════════════════
    // Private helpers
    // ══════════════════════════════════════════════════════════
    private async Task CreateRoleProfileAsync(AppUser user, RegisterDto dto)
    {
        if (dto.Role == UserRoles.Patient)
        {
            _ctx.Patients.Add(new Patient
            {
                UserId = user.Id,
                Gender = dto.Gender ?? "Unknown",
                DateOfBirth = dto.DateOfBirth ?? DateTime.UtcNow.AddYears(-25)
            });
        }
        else if (dto.Role == UserRoles.Doctor)
        {
            _ctx.Doctors.Add(new Doctor
            {
                UserId = user.Id,
                Specialty = dto.Specialty ?? "General",
                Qualifications = dto.Qualifications ?? string.Empty,
                ConsultationFee = dto.ConsultationFee ?? 500,
                IsVerified = false
            });
        }
        await _ctx.SaveChangesAsync();
    }

    private AuthResponseDto BuildAuthResponse(AppUser user) => new()
    {
        Token = _tokenService.GenerateToken(user),
        RefreshToken = _tokenService.GenerateRefreshToken(),
        Expiration = DateTime.UtcNow.AddMinutes(60),
        UserId = user.Id,
        Email = user.Email!,
        FullName = user.FullName,
        Role = user.Role,
        ProfilePicture = user.ProfilePicture
    };
}

// ── DTOs used only in AuthController ──────────────────────────
public record ForgotPasswordOtpRequestDto(string Email);
public record VerifyForgotPasswordOtpDto(string Email, string Otp, string NewPassword);