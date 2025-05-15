using FluentValidation;

namespace HealthcareManagement.Application.Doctors.Commands.RemoveSpecialty;

public class RemoveSpecialtyFromDoctorCommandValidator : AbstractValidator<RemoveSpecialtyFromDoctorCommand>
{
    public RemoveSpecialtyFromDoctorCommandValidator()
    {
        RuleFor(x => x.DoctorId)
               .NotEmpty().WithMessage("Id cannot be empty");

        RuleFor(x => x.Specialty)
               .NotEmpty().WithMessage("Specialty is required")
               .Length(3, 100).WithMessage("Specialty must have between 3 and 100 characters");
    }
}
