using DoctorBooking.API.Domain.Entities;

namespace DoctorBooking.API.Infrastructure.Services.Interfaces;

public interface ITokenService
{
    string CreateToken(AppUser user);
}