using FluentValidation;

namespace HealthcareManagement.Application.Doctors.Commands.CreateDoctor;

public class CreateDoctorCommandValidator : AbstractValidator<CreateDoctorCommand>
{
    public CreateDoctorCommandValidator()
    {
        RuleFor(d => d.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .Length(3, 100).WithMessage("First name must have between 3 and 100 characters");

        RuleFor(d => d.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .Length(3, 100).WithMessage("Last name must have between 3 and 100 characters");

        RuleFor(x => x.Birthdate)
            .NotEmpty().WithMessage("Birthdate is required")
            .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage("Birthdate must be in the past");

        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("CPF is required")
            .Length(11).WithMessage("CPF must have 11 characters")
            .Matches(@"^\d+$").WithMessage("CPF must contain only numbers");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .Length(10, 11).WithMessage("Phone number must have 10 or 11 characters")
            .Matches(@"^\d+$").WithMessage("Phone number must contain only numbers");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .MaximumLength(100).WithMessage("Email cannot exceed 100 characters")
            .EmailAddress().WithMessage("Invalid email");
    }
}
