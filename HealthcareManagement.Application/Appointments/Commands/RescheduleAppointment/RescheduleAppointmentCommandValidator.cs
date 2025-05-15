using FluentValidation;

namespace HealthcareManagement.Application.Appointments.Commands.RescheduleAppointment;

public class RescheduleAppointmentCommandValidator : AbstractValidator<RescheduleAppointmentCommand>
{
    public RescheduleAppointmentCommandValidator()
    {
        RuleFor(x => x.Id)
               .NotEmpty().WithMessage("Id cannot be empty");

        RuleFor(x => x.NewAppointmentDate)
            .Must(date => date > DateTime.Now)
                .WithMessage("Appointment date must be in the future")
            .Must(date => date.Hour > 8 && date.Hour < 17)
                .WithMessage("Appointment date must be between 8:00 and 17:00");
    }
}
