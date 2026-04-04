using DoctorBooking.API.Application.DTOs.Auth;
using FluentValidation;

namespace DoctorBooking.API.Application.Validators;

public class RegisterValidator : AbstractValidator<RegisterDto>
{
    public RegisterValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50);

        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress().WithMessage("Valid email is required.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one number.");

        RuleFor(x => x.Role)
            .Must(r => r is "Patient" or "Doctor")
            .WithMessage("Role must be Patient or Doctor.");

        // Doctor-specific rules
        When(x => x.Role == "Doctor", () =>
        {
            RuleFor(x => x.Specialty)
                .NotEmpty().WithMessage("Specialty is required for doctors.");
            RuleFor(x => x.ConsultationFee)
                .GreaterThan(0).WithMessage("Consultation fee must be greater than 0.");
        });
    }
}