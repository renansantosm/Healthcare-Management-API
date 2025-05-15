using FluentValidation;

namespace HealthcareManagement.Application.Appointments.Commands.CancelAppointment;

public class CancelAppointmentCommandValidator : AbstractValidator<CancelAppointmentCommand>
{
    public CancelAppointmentCommandValidator()
    {
        RuleFor(x => x.Id)
               .NotEmpty().WithMessage("Id cannot be empty");
    }
}

