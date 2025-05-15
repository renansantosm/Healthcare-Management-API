using FluentValidation;

namespace HealthcareManagement.Application.Doctors.Commands.RemoveDoctor;

public class RemoveDoctorCommandValidator : AbstractValidator<RemoveDoctorCommand>
{
    public RemoveDoctorCommandValidator()
    {
        RuleFor(d => d.Id)
               .NotEmpty().WithMessage("Id cannot be empty");
    }
}
