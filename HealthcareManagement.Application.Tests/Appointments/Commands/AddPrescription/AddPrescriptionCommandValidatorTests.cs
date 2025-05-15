using FluentValidation.TestHelper;
using HealthcareManagement.Application.Appointments.Commands.AddPrescription;

namespace HealthcareManagement.Application.Tests.Appointments.Commands.AddPrescription;

public class AddPrescriptionCommandValidatorTests
{
    private readonly AddPrescriptionCommandValidator _validator;

    public AddPrescriptionCommandValidatorTests()
    {
        _validator = new AddPrescriptionCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_AppointmentId_Is_Empty()
    {
        // Arrange
        var command = new AddPrescriptionCommand(
            Guid.Empty,
            "Medication",
            "Dosage",
            "Duration",
            "Instructions"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AppointmentId)
              .WithErrorMessage("Id cannot be empty");
    }

    [Fact]
    public void Should_Not_Have_Error_When_AppointmentId_Is_NotEmpty()
    {
        // Arrange
        var command = new AddPrescriptionCommand(
            Guid.NewGuid(),
            "Medication",
            "Dosage",
            "Duration",
            "Instructions"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.AppointmentId);
    }

    [Fact]
    public void Should_Have_Error_When_Medication_Is_Empty()
    {
        // Arrange
        var command = new AddPrescriptionCommand(
            Guid.NewGuid(),
            string.Empty,
            "Dosage",
            "Duration",
            "Instructions"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Medication)
              .WithErrorMessage("Medication is required");
    }

    [Fact]
    public void Should_Have_Error_When_Medication_Exceeds_MaxLength()
    {
        // Arrange
        var longMedication = new string('A', 101);
        var command = new AddPrescriptionCommand(
            Guid.NewGuid(),
            longMedication,
            "Dosage",
            "Duration",
            "Instructions"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Medication)
              .WithErrorMessage("Medication cannot exceed 100 characters");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Medication_Is_Valid()
    {
        // Arrange
        var command = new AddPrescriptionCommand(
            Guid.NewGuid(),
            "Amoxicillin",
            "Dosage",
            "Duration",
            "Instructions"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Medication);
    }

    [Fact]
    public void Should_Have_Error_When_Dosage_Is_Empty()
    {
        // Arrange
        var command = new AddPrescriptionCommand(
            Guid.NewGuid(),
            "Medication",
            string.Empty,
            "Duration",
            "Instructions"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dosage)
              .WithErrorMessage("Dosage is required");
    }

    [Fact]
    public void Should_Have_Error_When_Dosage_Exceeds_MaxLength()
    {
        // Arrange
        var longDosage = new string('B', 51);
        var command = new AddPrescriptionCommand(
            Guid.NewGuid(),
            "Medication",
            longDosage,
            "Duration",
            "Instructions"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Dosage)
              .WithErrorMessage("Dosage cannot exceed 50 characters");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Dosage_Is_Valid()
    {
        // Arrange
        var command = new AddPrescriptionCommand(
            Guid.NewGuid(),
            "Medication",
            "500mg",
            "Duration",
            "Instructions"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Dosage);
    }

    [Fact]
    public void Should_Have_Error_When_Duration_Is_Empty()
    {
        // Arrange
        var command = new AddPrescriptionCommand(
            Guid.NewGuid(),
            "Medication",
            "Dosage",
            string.Empty,
            "Instructions"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Duration)
              .WithErrorMessage("Duration is required");
    }

    [Fact]
    public void Should_Have_Error_When_Duration_Exceeds_MaxLength()
    {
        // Arrange
        var longDuration = new string('C', 51);
        var command = new AddPrescriptionCommand(
            Guid.NewGuid(),
            "Medication",
            "Dosage",
            longDuration,
            "Instructions"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Duration)
              .WithErrorMessage("Duration cannot exceed 50 characters");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Duration_Is_Valid()
    {
        // Arrange
        var command = new AddPrescriptionCommand(
            Guid.NewGuid(),
            "Medication",
            "Dosage",
            "7 days",
            "Instructions"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Duration);
    }

    [Fact]
    public void Should_Have_Error_When_Instructions_Is_Empty()
    {
        // Arrange
        var command = new AddPrescriptionCommand(
            Guid.NewGuid(),
            "Medication",
            "Dosage",
            "Duration",
            string.Empty
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Instructions)
              .WithErrorMessage("Instructions are required");
    }

    [Fact]
    public void Should_Have_Error_When_Instructions_Exceeds_MaxLength()
    {
        // Arrange
        var longInstructions = new string('D', 501);
        var command = new AddPrescriptionCommand(
            Guid.NewGuid(),
            "Medication",
            "Dosage",
            "Duration",
            longInstructions
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Instructions)
              .WithErrorMessage("Instructions cannot exceed 500 characters");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Instructions_Is_Valid()
    {
        // Arrange
        var command = new AddPrescriptionCommand(
            Guid.NewGuid(),
            "Medication",
            "Dosage",
            "Duration",
            "Take one pill daily"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Instructions);
    }
}
