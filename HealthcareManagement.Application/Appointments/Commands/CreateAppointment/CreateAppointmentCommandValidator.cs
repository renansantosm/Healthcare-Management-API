using FluentValidation;

namespace HealthcareManagement.Application.Appointments.Commands.Create;

public class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentCommandValidator()
    {
        RuleFor(x => x.DoctorId)
               .NotEmpty().WithMessage("Id cannot be empty");

        RuleFor(x => x.PatientId)
               .NotEmpty().WithMessage("Id cannot be empty");

        RuleFor(x => x.AppointmentDate)
            .NotEqual(DateTimeOffset.MinValue)
                .WithMessage("Invalid appointment date")
            .Must(date => date > DateTimeOffset.UtcNow)
                .WithMessage("Appointment date must be in the future")
            .Must(date => date.Hour > 8 && date.Hour < 17)
                .WithMessage("Appointment date must be between 8:00 and 17:00");
    }
}

