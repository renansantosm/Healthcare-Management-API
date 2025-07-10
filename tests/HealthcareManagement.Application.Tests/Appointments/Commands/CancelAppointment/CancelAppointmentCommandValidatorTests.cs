using FluentValidation.TestHelper;
using HealthcareManagement.Application.Appointments.Commands.CancelAppointment;

namespace HealthcareManagement.Application.Tests.Appointments.Commands.CancelAppointment;

public class CancelAppointmentCommandValidatorTests
{
    private readonly CancelAppointmentCommandValidator _validator;

    public CancelAppointmentCommandValidatorTests()
    {
        _validator = new CancelAppointmentCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Id_Is_Empty()
    {
        // Arrange
        var command = new CancelAppointmentCommand(Guid.Empty);

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
        var command = new CancelAppointmentCommand(Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}
