using FluentValidation.TestHelper;
using HealthcareManagement.Application.Appointments.Commands.Create;

namespace HealthcareManagement.Application.Tests.Appointments.Commands.CreateAppointment;

public class CreateAppointmentCommandValidatorTests
{
    private readonly CreateAppointmentCommandValidator _validator;

    public CreateAppointmentCommandValidatorTests()
    {
        _validator = new CreateAppointmentCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_DoctorId_Is_Empty()
    {
        // Arrange
        var command = new CreateAppointmentCommand(
            Guid.Empty,
            Guid.NewGuid(),
            DateTimeOffset.UtcNow.AddDays(1).Date.AddHours(9)
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
        var command = new CreateAppointmentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTimeOffset.UtcNow.AddDays(1).Date.AddHours(9)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.DoctorId);
    }

    [Fact]
    public void Should_Have_Error_When_PatientId_Is_Empty()
    {
        // Arrange
        var command = new CreateAppointmentCommand(
            Guid.NewGuid(),
            Guid.Empty,
            DateTimeOffset.UtcNow.AddDays(1).Date.AddHours(9)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PatientId)
              .WithErrorMessage("Id cannot be empty");
    }

    [Fact]
    public void Should_Not_Have_Error_When_PatientId_Is_NotEmpty()
    {
        // Arrange
        var command = new CreateAppointmentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTimeOffset.UtcNow.AddDays(1).Date.AddHours(9)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PatientId);
    }

    [Fact]
    public void Should_Have_Error_When_AppointmentDate_Is_MinValue()
    {
        // Arrange
        var command = new CreateAppointmentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTimeOffset.MinValue
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AppointmentDate)
              .WithErrorMessage("Invalid appointment date");
    }

    [Fact]
    public void Should_Have_Error_When_AppointmentDate_Is_In_The_Past()
    {
        // Arrange
        var command = new CreateAppointmentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTimeOffset.UtcNow.AddDays(-1)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AppointmentDate)
              .WithErrorMessage("Appointment date must be in the future");
    }

    [Fact]
    public void Should_Have_Error_When_AppointmentDate_Hour_Is_8()
    {
        // Arrange
        var command = new CreateAppointmentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTimeOffset.UtcNow.AddDays(1).Date.AddHours(8)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AppointmentDate)
              .WithErrorMessage("Appointment date must be between 8:00 and 17:00");
    }

    [Fact]
    public void Should_Have_Error_When_AppointmentDate_Hour_Is_17()
    {
        // Arrange
        var command = new CreateAppointmentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTimeOffset.UtcNow.AddDays(1).Date.AddHours(17)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AppointmentDate)
              .WithErrorMessage("Appointment date must be between 8:00 and 17:00");
    }

    [Fact]
    public void Should_Not_Have_Error_When_AppointmentDate_Is_Valid()
    {
        // Arrange
        var command = new CreateAppointmentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTimeOffset.UtcNow.AddDays(1).Date.AddHours(9)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.AppointmentDate);
    }
}
