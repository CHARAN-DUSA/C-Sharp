namespace DoctorBooking.API.Application.DTOs.Auth;

public class TwoFactorSetupResponseDto
{
    public string Secret { get; set; } = string.Empty;
    public string QrCodeImageUrl { get; set; } = string.Empty;
    public string ManualEntryKey { get; set; } = string.Empty;
}

public class TwoFactorVerifyDto
{
    public string Code { get; set; } = string.Empty;
}

public class TwoFactorLoginDto
{
    public string UserId { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}