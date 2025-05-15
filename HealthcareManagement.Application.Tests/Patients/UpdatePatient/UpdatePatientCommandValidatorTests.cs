using FluentValidation.TestHelper;
using HealthcareManagement.Application.Doctors.Commands.UpdateDoctor;
using HealthcareManagement.Application.Patients.Commands.UpdatePatient;

namespace HealthcareManagement.Application.Tests.Patients.UpdatePatient;

public class UpdatePatientCommandValidatorTests
{
    private readonly UpdatePatientCommandValidator _validator;

    public UpdatePatientCommandValidatorTests()
    {
        _validator = new UpdatePatientCommandValidator();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        // Arrange
        var command = new UpdatePatientCommand
        (
            Guid.NewGuid(),
            "John",
            "Doe",
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30)),
            "12345678901",
            "11987654321",
            "john.doe@example.com"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Have_Error_When_Id_Is_Empty()
    {
        // Arrange
        var command = new UpdatePatientCommand
        (
            Guid.Empty,
            "John",
            "Doe",
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30)),
            "12345678901",
            "11987654321",
            "john.doe@example.com"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Id cannot be empty");
    }

    [Fact]
    public void Should_Have_Error_When_FirstName_Is_Empty()
    {
        // Arrange
        var command = new UpdatePatientCommand
        (
            Guid.NewGuid(),
            "",
            "Doe",
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30)),
            "12345678901",
            "11987654321",
            "john.doe@example.com"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("First name is required");
    }

    [Fact]
    public void Should_Have_Error_When_FirstName_Is_Too_Short()
    {
        // Arrange
        var command = new UpdatePatientCommand
        (
            Guid.NewGuid(),
            "Jo",
            "Doe",
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30)),
            "12345678901",
            "11987654321",
            "john.doe@example.com"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("First name must have between 3 and 100 characters");
    }

    [Fact]
    public void Should_Have_Error_When_FirstName_Is_Too_Long()
    {
        // Arrange
        var command = new UpdatePatientCommand
        (
            Guid.NewGuid(),
            new string('A', 101),
            "Doe",
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30)),
            "12345678901",
            "11987654321",
            "john.doe@example.com"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("First name must have between 3 and 100 characters");
    }

    [Fact]
    public void Should_Have_Error_When_LastName_Is_Empty()
    {
        // Arrange
        var command = new UpdatePatientCommand
        (
            Guid.NewGuid(),
            "John",
            "",
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30)),
            "12345678901",
            "11987654321",
            "john.doe@example.com"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name is required");
    }

    [Fact]
    public void Should_Have_Error_When_LastName_Is_Too_Short()
    {
        // Arrange
        var command = new UpdatePatientCommand
        (
            Guid.NewGuid(),
            "John",
            "Do",
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30)),
            "12345678901",
            "11987654321",
            "john.doe@example.com"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name must have between 3 and 100 characters");
    }

    [Fact]
    public void Should_Have_Error_When_LastName_Is_Too_Long()
    {
        // Arrange
        var command = new UpdatePatientCommand
        (
            Guid.NewGuid(),
            "John",
            new string('A', 101),
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30)),
            "12345678901",
            "11987654321",
            "john.doe@example.com"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name must have between 3 and 100 characters");
    }

    [Fact]
    public void Should_Have_Error_When_Birthdate_Is_Empty()
    {
        // Arrange
        var command = new UpdatePatientCommand
        (
            Guid.NewGuid(),
            "John",
            "Doe",
            default,
            "12345678901",
            "11987654321",
            "john.doe@example.com"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BirthDate)
            .WithErrorMessage("Birthdate is required");
    }

    [Fact]
    public void Should_Have_Error_When_Birthdate_Is_In_Future()
    {
        // Arrange
        var command = new UpdatePatientCommand
        (
            Guid.NewGuid(),
            "John",
            "Doe",
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            "12345678901",
            "11987654321",
            "john.doe@example.com"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BirthDate)
            .WithErrorMessage("Birthdate must be in the past");
    }

    [Fact]
    public void Should_Have_Error_When_Cpf_Is_Empty()
    {
        // Arrange
        var command = new UpdatePatientCommand
        (
            Guid.NewGuid(),
            "John",
            "Doe",
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30)),
            "",
            "11987654321",
            "john.doe@example.com"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Cpf)
            .WithErrorMessage("CPF is required");
    }

    [Fact]
    public void Should_Have_Error_When_Cpf_Length_Is_Invalid()
    {
        // Arrange
        var command = new UpdatePatientCommand
        (
            Guid.NewGuid(),
            "John",
            "Doe",
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30)),
            "1234567890",
            "11987654321",
            "john.doe@example.com"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Cpf)
            .WithErrorMessage("CPF must have exactly 11 digits");
    }

    [Fact]
    public void Should_Have_Error_When_Cpf_Contains_Non_Numeric_Characters()
    {
        // Arrange
        var command = new UpdatePatientCommand
        (
            Guid.NewGuid(),
            "John",
            "Doe",
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30)),
            "1234567890A",
            "11987654321",
            "john.doe@example.com"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Cpf)
            .WithErrorMessage("CPF must contain only numbers");
    }

    [Fact]
    public void Should_Have_Error_When_PhoneNumber_Is_Empty()
    {
        // Arrange
        var command = new UpdatePatientCommand
        (
            Guid.NewGuid(),
            "John",
            "Doe",
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30)),
            "12345678901",
            "",
            "john.doe@example.com"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("Phone number is required");
    }

    [Fact]
    public void Should_Have_Error_When_PhoneNumber_Is_Too_Short()
    {
        // Arrange
        var command = new UpdatePatientCommand
        (
            Guid.NewGuid(),
            "John",
            "Doe",
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30)),
            "12345678901",
            "123456789",
            "john.doe@example.com"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("Phone number must have 10 or 11 characters");
    }

    [Fact]
    public void Should_Have_Error_When_PhoneNumber_Is_Too_Long()
    {
        // Arrange
        var command = new UpdatePatientCommand
        (
            Guid.NewGuid(),
            "John",
            "Doe",
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30)),
            "12345678901",
            "123456789012",
            "john.doe@example.com"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("Phone number must have 10 or 11 characters");
    }

    [Fact]
    public void Should_Have_Error_When_PhoneNumber_Contains_Non_Numeric_Characters()
    {
        // Arrange
        var command = new UpdatePatientCommand
        (
            Guid.NewGuid(),
            "John",
            "Doe",
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30)),
            "12345678901",
            "1198765432A",
            "john.doe@example.com"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("Phone number must contain only numbers");
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        // Arrange
        var command = new UpdatePatientCommand
        (
            Guid.NewGuid(),
            "John",
            "Doe",
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30)),
            "12345678901",
            "11987654321",
            ""
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is required");
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Too_Long()
    {
        // Arrange
        var command = new UpdatePatientCommand
        (
            Guid.NewGuid(),
            "John",
            "Doe",
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30)),
            "12345678901",
            "11987654321",
            $"{new string('a', 90)}@example.com"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email cannot exceed 100 characters");
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid()
    {
        // Arrange
        var command = new UpdatePatientCommand
        (
            Guid.NewGuid(),
            "John",
            "Doe",
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30)),
            "12345678901",
            "11987654321",
            "invalid-email"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Invalid email");
    }
}

