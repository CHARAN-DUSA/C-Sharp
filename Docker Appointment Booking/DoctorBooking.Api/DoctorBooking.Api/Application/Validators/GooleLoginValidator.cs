using DoctorBooking.API.Application.DTOs.Auth;
using FluentValidation;

namespace DoctorBooking.API.Application.Validators;

public class GoogleLoginValidator : AbstractValidator<GoogleLoginDto>
{
    public GoogleLoginValidator()
    {
        RuleFor(x => x.IdToken)
            .NotEmpty().WithMessage("Google ID token is required.");

        RuleFor(x => x.Role)
            .Must(r => r is "Patient" or "Doctor")
            .WithMessage("Role must be Patient or Doctor.");
    }
}