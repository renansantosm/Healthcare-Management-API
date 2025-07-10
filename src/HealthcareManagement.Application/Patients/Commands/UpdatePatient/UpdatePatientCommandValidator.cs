using FluentValidation;

namespace HealthcareManagement.Application.Patients.Commands.UpdatePatient;

public class UpdatePatientCommandValidator : AbstractValidator<UpdatePatientCommand>
{
    public UpdatePatientCommandValidator()
    {
        RuleFor(x => x.Id)
               .NotEmpty().WithMessage("Id cannot be empty");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .Length(3, 100).WithMessage("First name must have between 3 and 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .Length(3, 100).WithMessage("Last name must have between 3 and 100 characters");

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("Birthdate is required")
            .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage("Birthdate must be in the past");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .Length(10, 11).WithMessage("Phone number must have 10 or 11 characters")
            .Matches(@"^\d+$").WithMessage("Phone number must contain only numbers");

        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("CPF is required")
            .Length(11).WithMessage("CPF must have exactly 11 digits")
            .Matches(@"^\d+$").WithMessage("CPF must contain only numbers");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .MaximumLength(100).WithMessage("Email cannot exceed 100 characters")
            .EmailAddress().WithMessage("Invalid email");
    }
}
