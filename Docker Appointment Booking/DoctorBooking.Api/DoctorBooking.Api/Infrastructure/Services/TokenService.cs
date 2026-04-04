using DoctorBooking.API.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DoctorBooking.API.Infrastructure.Services;

public class TokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(AppUser user)
    {
        var jwt = _config.GetSection("JwtSettings");

        var secret = jwt["Secret"];
        if (string.IsNullOrEmpty(secret))
            throw new Exception("JWT Secret missing");

        var issuer = jwt["Issuer"];
        var audience = jwt["Audience"];
        var expiryMinutes = jwt["ExpirationInMinutes"];

        if (string.IsNullOrEmpty(expiryMinutes))
            throw new Exception("JWT ExpirationInMinutes missing");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddMinutes(double.Parse(expiryMinutes));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.FullName ?? ""),
            new Claim(ClaimTypes.Role, user.Role ?? "")
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    public DateTime GetRefreshExpiry()
    {
        var days = _config["JwtSettings:RefreshTokenExpirationInDays"];

        if (string.IsNullOrEmpty(days))
            throw new Exception("RefreshTokenExpirationInDays missing");

        return DateTime.UtcNow.AddDays(double.Parse(days));
    }
}