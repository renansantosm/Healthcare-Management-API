using FluentValidation.TestHelper;
using HealthcareManagement.Application.Patients.Commands.RemovePatient;

namespace HealthcareManagement.Application.Tests.Patients.RemovePatient;

public class RemovePatientCommandValidatorTests
{
    private readonly RemovePatientCommandValidator _validator;

    public RemovePatientCommandValidatorTests()
    {
        _validator = new RemovePatientCommandValidator();
    }

    [Fact]
    public void Validate_ShouldNotReturnError_WhenIdIsValid()
    {
        // Arrange
        var command = new RemovePatientCommand(Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenIdIsEmpty()
    {
        // Arrange
        var command = new RemovePatientCommand(Guid.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Id cannot be empty");
    }
}
