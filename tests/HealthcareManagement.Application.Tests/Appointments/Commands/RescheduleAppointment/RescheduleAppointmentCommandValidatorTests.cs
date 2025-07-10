using FluentValidation.TestHelper;
using HealthcareManagement.Application.Appointments.Commands.RescheduleAppointment;

namespace HealthcareManagement.Application.Tests.Appointments.Commands.RescheduleAppointment;

public class RescheduleAppointmentCommandValidatorTests
{
    private readonly RescheduleAppointmentCommandValidator _validator;

    public RescheduleAppointmentCommandValidatorTests()
    {
        _validator = new RescheduleAppointmentCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Id_Is_Empty()
    {
        // Arrange
        var command = new RescheduleAppointmentCommand(
            Guid.Empty,
            DateTime.Now.AddDays(1).AddHours(9)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
              .WithErrorMessage("Id cannot be empty");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Id_Is_NotEmpty()
    {
        // Arrange
        var command = new RescheduleAppointmentCommand(
            Guid.NewGuid(),
            DateTime.Now.AddDays(1).AddHours(9)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Should_Have_Error_When_NewAppointmentDate_Is_In_The_Past()
    {
        // Arrange
        var command = new RescheduleAppointmentCommand(
            Guid.NewGuid(),
            DateTime.Now.AddHours(-1)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewAppointmentDate)
              .WithErrorMessage("Appointment date must be in the future");
    }

    [Fact]
    public void Should_Have_Error_When_NewAppointmentDate_Hour_Is_8()
    {
        // Arrange
        var command = new RescheduleAppointmentCommand(
            Guid.NewGuid(),
             DateTime.Now.Date.AddDays(1).AddHours(8)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewAppointmentDate)
              .WithErrorMessage("Appointment date must be between 8:00 and 17:00");
    }

    [Fact]
    public void Should_Have_Error_When_NewAppointmentDate_Hour_Is_17()
    {
        // Arrange
        var command = new RescheduleAppointmentCommand(
            Guid.NewGuid(),
            DateTime.Now.Date.AddDays(1).AddHours(17)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewAppointmentDate)
              .WithErrorMessage("Appointment date must be between 8:00 and 17:00");
    }

    [Fact]
    public void Should_Not_Have_Error_When_NewAppointmentDate_Is_Valid()
    {
        // Arrange
        var command = new RescheduleAppointmentCommand(
            Guid.NewGuid(),
            DateTime.Now.Date.AddDays(1).AddHours(9)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.NewAppointmentDate);
    }
}
