using FluentValidation;

namespace HealthcareManagement.Application.Patients.Commands.RemovePatient;

public class RemovePatientCommandValidator : AbstractValidator<RemovePatientCommand>
{
    public RemovePatientCommandValidator()
    {
        RuleFor(x => x.Id)
               .NotEmpty().WithMessage("Id cannot be empty");
    }
}
