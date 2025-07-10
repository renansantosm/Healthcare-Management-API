using FluentValidation;

namespace HealthcareManagement.Application.Appointments.Commands.AddPrescription;

public class AddPrescriptionCommandValidator : AbstractValidator<AddPrescriptionCommand>
{
    public AddPrescriptionCommandValidator()
    {
        RuleFor(x => x.AppointmentId)
            .NotEmpty().WithMessage("Id cannot be empty");

        RuleFor(x => x.Medication)
            .NotEmpty().WithMessage("Medication is required")
            .MaximumLength(100).WithMessage("Medication cannot exceed 100 characters");

        RuleFor(x => x.Dosage)
            .NotEmpty().WithMessage("Dosage is required")
            .MaximumLength(50).WithMessage("Dosage cannot exceed 50 characters");

        RuleFor(x => x.Duration)
            .NotEmpty().WithMessage("Duration is required")
            .MaximumLength(50).WithMessage("Duration cannot exceed 50 characters");

        RuleFor(x => x.Instructions)
            .NotEmpty().WithMessage("Instructions are required")
            .MaximumLength(500).WithMessage("Instructions cannot exceed 500 characters");
    }
}
