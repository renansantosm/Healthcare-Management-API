using FluentValidation.TestHelper;
using HealthcareManagement.Application.Doctors.Commands.RemoveDoctor;
using HealthcareManagement.Application.Patients.Commands.RemovePatient;

namespace HealthcareManagement.Application.Tests.Doctors.Commands.RemoveDoctor;

public class RemoveDoctorCommandValidatorTests
{
    private readonly RemoveDoctorCommandValidator _validator;

    public RemoveDoctorCommandValidatorTests()
    {
        _validator = new RemoveDoctorCommandValidator();
    }

    [Fact]
    public void Validate_ShouldNotReturnError_WhenIdIsValid()
    {
        // Arrange
        var command = new RemoveDoctorCommand(Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenIdIsEmpty()
    {
        // Arrange
        var command = new RemoveDoctorCommand(Guid.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Id cannot be empty");
    }
}
