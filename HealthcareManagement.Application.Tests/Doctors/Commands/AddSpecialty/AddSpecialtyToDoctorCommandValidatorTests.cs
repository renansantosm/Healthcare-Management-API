using FluentValidation.TestHelper;
using HealthcareManagement.Application.Doctors.Commands.AddSpecialty;

namespace HealthcareManagement.Application.Tests.Doctors.Commands.AddSpecialty;

public class AddSpecialtyToDoctorCommandValidatorTests
{
    private readonly AddSpecialtyToDoctorCommandValidator _validator;

    public AddSpecialtyToDoctorCommandValidatorTests()
    {
        _validator = new AddSpecialtyToDoctorCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_DoctorId_Is_Empty()
    {
        // Arrange
        var command = new AddSpecialtyToDoctorCommand(
            Guid.Empty,
            "Cardiology"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DoctorId)
              .WithErrorMessage("Id cannot be empty");
    }

    [Fact]
    public void Should_Not_Have_Error_When_DoctorId_Is_NotEmpty()
    {
        // Arrange
        var command = new AddSpecialtyToDoctorCommand(
            Guid.NewGuid(),
            "Cardiology"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.DoctorId);
    }

    [Fact]
    public void Should_Have_Error_When_Specialty_Is_Empty()
    {
        // Arrange
        var command = new AddSpecialtyToDoctorCommand(
            Guid.NewGuid(),
            string.Empty
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Specialty)
              .WithErrorMessage("Specialty is required");
    }

    [Fact]
    public void Should_Have_Error_When_Specialty_Is_Too_Short()
    {
        // Arrange
        var shortSpecialty = new string('A', 2);
        var command = new AddSpecialtyToDoctorCommand(
            Guid.NewGuid(),
            shortSpecialty
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Specialty)
              .WithErrorMessage("Specialty must have between 3 and 100 characters");
    }

    [Fact]
    public void Should_Have_Error_When_Specialty_Is_Too_Long()
    {
        // Arrange
        var longSpecialty = new string('B', 101);
        var command = new AddSpecialtyToDoctorCommand(
            Guid.NewGuid(),
            longSpecialty
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Specialty)
              .WithErrorMessage("Specialty must have between 3 and 100 characters");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Specialty_Is_MinLength()
    {
        // Arrange
        var minSpecialty = new string('C', 3);
        var command = new AddSpecialtyToDoctorCommand(
            Guid.NewGuid(),
            minSpecialty
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Specialty);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Specialty_Is_MaxLength()
    {
        // Arrange
        var maxSpecialty = new string('D', 100);
        var command = new AddSpecialtyToDoctorCommand(
            Guid.NewGuid(),
            maxSpecialty
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Specialty);
    }
}
