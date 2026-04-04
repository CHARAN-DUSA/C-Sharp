using DoctorBooking.API.Application.DTOs.Appointment;
using FluentValidation;

namespace DoctorBooking.API.Application.Validators;

public class BookAppointmentValidator : AbstractValidator<BookAppointmentDto>
{
    public BookAppointmentValidator()
    {
        RuleFor(x => x.DoctorId)
            .GreaterThan(0).WithMessage("Valid DoctorId is required.");

        RuleFor(x => x.AppointmentDate)
            .Must(d => d.Date >= DateTime.UtcNow.Date)
            .WithMessage("Appointment date must be today or in the future.");

        RuleFor(x => x.TimeSlot)
            .NotEmpty().WithMessage("Time slot is required.")
            .Matches(@"^\d{2}:\d{2}$").WithMessage("Time slot must be in HH:mm format.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required.")
            .MaximumLength(500).WithMessage("Reason must not exceed 500 characters.");
    }
}