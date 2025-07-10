using FluentValidation.TestHelper;
using HealthcareManagement.Application.Doctors.Commands.RemoveSpecialty;

namespace HealthcareManagement.Application.Tests.Doctors.Commands.RemoveSpecialty;

public class RemoveSpecialtyFromDoctorCommandValidatorTests
{
    private readonly RemoveSpecialtyFromDoctorCommandValidator _validator;
    public RemoveSpecialtyFromDoctorCommandValidatorTests()
    {
        _validator = new RemoveSpecialtyFromDoctorCommandValidator();
    }

    [Fact]
    public void Validate_ShouldNotReturnError_WhenDoctorIdIsValid()
    {
        // Arrange
        var command = new RemoveSpecialtyFromDoctorCommand(Guid.NewGuid(), "Cardiology");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.DoctorId);
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenDoctorIdIsEmpty()
    {
        // Arrange
        var command = new RemoveSpecialtyFromDoctorCommand(Guid.Empty, "Cardiology");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DoctorId)
            .WithErrorMessage("Id cannot be empty");
    }

    [Fact]
    public void Validate_ShouldNotReturnError_WhenSpecialtyIsValid()
    {
        // Arrange
        var command = new RemoveSpecialtyFromDoctorCommand(Guid.NewGuid(), "Cardiology");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Specialty);
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenSpecialtyIsEmpty()
    {
        // Arrange
        var command = new RemoveSpecialtyFromDoctorCommand(Guid.NewGuid(), string.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Specialty)
            .WithErrorMessage("Specialty is required");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenSpecialtyIsTooShort()
    {
        // Arrange
        var command = new RemoveSpecialtyFromDoctorCommand(Guid.NewGuid(), "AB");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Specialty)
            .WithErrorMessage("Specialty must have between 3 and 100 characters");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenSpecialtyIsTooLong()
    {
        // Arrange
        var command = new RemoveSpecialtyFromDoctorCommand(Guid.NewGuid(), new string('A', 101));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Specialty)
            .WithErrorMessage("Specialty must have between 3 and 100 characters");
    }
}
