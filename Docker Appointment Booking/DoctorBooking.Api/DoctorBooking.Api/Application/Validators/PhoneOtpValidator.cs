//using DoctorBooking.API.Application.DTOs.Auth;
//using FluentValidation;

//namespace DoctorBooking.API.Application.Validators;

//public class SendOtpValidator : AbstractValidator<SendOtpDto>
//{
//    public SendOtpValidator()
//    {
//        RuleFor(x => x.PhoneNumber)
//            .NotEmpty().WithMessage("Phone number is required.")
//            .Matches(@"^\+[1-9]\d{7,14}$")
//            .WithMessage("Phone number must be in E.164 format, e.g. +919876543210.");
//    }
//}

//public class VerifyOtpValidator : AbstractValidator<VerifyOtpDto>
//{
//    public VerifyOtpValidator()
//    {
//        RuleFor(x => x.PhoneNumber)
//            .NotEmpty()
//            .Matches(@"^\+[1-9]\d{7,14}$").WithMessage("Invalid phone number format.");

//        RuleFor(x => x.OtpCode)
//            .NotEmpty().WithMessage("OTP code is required.")
//            .Length(6).WithMessage("OTP must be exactly 6 digits.")
//            .Matches(@"^\d{6}$").WithMessage("OTP must contain digits only.");
//    }
//}