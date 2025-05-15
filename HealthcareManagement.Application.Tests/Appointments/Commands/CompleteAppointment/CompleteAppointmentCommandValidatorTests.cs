using FluentValidation.TestHelper;
using HealthcareManagement.Application.Appointments.Commands.Complete;

namespace HealthcareManagement.Application.Tests.Appointments.Commands.CompleteAppointment;

public class CompleteAppointmentCommandValidatorTests
{
    private readonly CompleteAppointmentCommandValidator _validator;
    public CompleteAppointmentCommandValidatorTests()
    {
        _validator = new CompleteAppointmentCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Id_Is_Empty()
    {
        // Arrange
        var command = new CompleteAppointmentCommand(Guid.Empty);

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
        var command = new CompleteAppointmentCommand(Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}
