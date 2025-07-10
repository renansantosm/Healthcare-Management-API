using HealthcareManagement.Application.Doctors.Commands.UpdateDoctor;
using HealthcareManagement.Application.Patients.Commands.CreatePatient;
using HealthcareManagement.Application.Patients.Commands.UpdatePatient;
using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Application.Services.Validation;
using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Exceptions.Doctor;
using HealthcareManagement.Domain.Exceptions.Shared;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Domain.ValueObjects;
using MediatR;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace HealthcareManagement.Application.Tests.Patients.UpdatePatient;

public class UpdatePatientCommandHandlerTests
{
    private readonly IPatientRepository _patientRepository;
    private readonly IPatientValidationService _patientValidationService;
    private readonly UpdatePatientCommandHandler _handler;
    private readonly PersonInfo _initialPersonInfo = PersonInfo.Create(
        FullName.Create("John", "Doe"),
        BirthDate.Create(new DateOnly(1990, 03, 01)),
        Cpf.Create("12345678909"),
        Email.Create("john_doe@email.com"),
        MobilePhoneNumber.Create("11987654321"));

    public UpdatePatientCommandHandlerTests()
    {
        _patientRepository = Substitute.For<IPatientRepository>();
        _patientValidationService = Substitute.For<IPatientValidationService>();
        _handler = new UpdatePatientCommandHandler(_patientRepository, _patientValidationService);
    }

    public async Task Handle_ValidCommand_UpdatesPatientAndReturnsUnit()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var existingPatient = new Patient(patientId, _initialPersonInfo);
        _patientValidationService.GetPatientOrThrowAsync(patientId).Returns(existingPatient);
        _patientValidationService.CheckCpfUniqueAsync(Arg.Any<string>()).Returns(Task.CompletedTask);
        _patientValidationService.CheckEmailUniqueAsync(Arg.Any<string>()).Returns(Task.CompletedTask);

        Patient updatedPatient = null;
        _patientRepository.UpdateAsync(Arg.Do<Patient>(p => updatedPatient = p)).Returns(Task.CompletedTask);

        var command = new UpdatePatientCommand(patientId, "John", "Doe", new DateOnly(1988, 5, 15), "12345678910", "11912345678", "doe_john@email.com");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Unit.Value, result);
        await _patientValidationService.Received(1).GetPatientOrThrowAsync(patientId);
        await _patientValidationService.Received(1).CheckCpfUniqueAsync(command.Cpf);
        await _patientValidationService.Received(1).CheckEmailUniqueAsync(command.Email);
        await _patientRepository.Received(1).UpdateAsync(existingPatient);

        Assert.Equal(command.FirstName, updatedPatient.PersonInfo.FullName.FirstName);
        Assert.Equal(command.LastName, updatedPatient.PersonInfo.FullName.LastName);
        Assert.Equal(command.BirthDate, updatedPatient.PersonInfo.BirthDate.Date);
        Assert.Equal(command.Cpf, updatedPatient.PersonInfo.Cpf.Number);
        Assert.Equal(command.Email, updatedPatient.PersonInfo.Email.Adress);
        Assert.Equal(command.PhoneNumber, updatedPatient.PersonInfo.MobilePhoneNumber.Number);
    }

    [Fact]
    public async Task Handle_PatientNotFound_ThrowsPatientNotFoundException_AndDoesNotCallUpdate()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        _patientValidationService.GetPatientOrThrowAsync(patientId)
            .Returns(Task.FromException<Patient>(new PatientNotFoundException($"PAtient with ID {patientId} not found.")));

        var command = new UpdatePatientCommand(patientId, "John", "Doe", new DateOnly(1988, 5, 15), "12345678910", "11912345678", "doe_john@email.com");

        // Act & Assert
        await Assert.ThrowsAsync<PatientNotFoundException>(() => _handler.Handle(command, CancellationToken.None));

        await _patientValidationService.Received(1).GetPatientOrThrowAsync(patientId);
        await _patientValidationService.DidNotReceive().CheckCpfUniqueAsync(Arg.Any<string>());
        await _patientValidationService.DidNotReceive().CheckEmailUniqueAsync(Arg.Any<string>());
        await _patientRepository.DidNotReceive().UpdateAsync(Arg.Any<Patient>());
    }

    [Fact]
    public async Task Handle_CpfNotUnique_ThrowsCpfNotUniqueException_AndDoesNotCallUpdate()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var existingPatient = new Patient(patientId, _initialPersonInfo);
        _patientValidationService.GetPatientOrThrowAsync(patientId).Returns(existingPatient);

        _patientValidationService.CheckCpfUniqueAsync(Arg.Any<string>())
            .ThrowsAsync(new CpfNotUniqueException("Cpf must be unique."));

        var command = new UpdatePatientCommand(patientId, "John", "Doe", new DateOnly(1988, 5, 15), "12345678910", "11912345678", "doe_john@email.com");

        // Act & Assert
        await Assert.ThrowsAsync<CpfNotUniqueException>(() => _handler.Handle(command, CancellationToken.None));

        await _patientValidationService.Received(1).GetPatientOrThrowAsync(patientId);
        await _patientValidationService.Received(1).CheckCpfUniqueAsync(command.Cpf);
        await _patientValidationService.DidNotReceive().CheckEmailUniqueAsync(Arg.Any<string>());
        await _patientRepository.DidNotReceive().UpdateAsync(Arg.Any<Patient>());
    }

    [Fact]
    public async Task Handle_EmailNotUnique_ThrowsEmailNotUniqueException_AndDoesNotCallUpdate()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var existingPatient = new Patient(patientId, _initialPersonInfo);

        _patientValidationService.GetPatientOrThrowAsync(patientId).Returns(existingPatient);
        _patientValidationService.CheckCpfUniqueAsync(Arg.Any<string>()).Returns(Task.CompletedTask);
        _patientValidationService.CheckEmailUniqueAsync(Arg.Any<string>())
            .ThrowsAsync(new EmailNotUniqueException("Email must be unique."));

        var command = new UpdatePatientCommand(patientId, "John", "Doe", new DateOnly(1988, 5, 15), "12345678910", "11912345678", "doe_john@email.com");

        // Act & Assert
        await Assert.ThrowsAsync<EmailNotUniqueException>(() => _handler.Handle(command, CancellationToken.None));

        await _patientValidationService.Received(1).GetPatientOrThrowAsync(patientId);
        await _patientValidationService.Received(1).CheckCpfUniqueAsync(command.Cpf);
        await _patientValidationService.Received(1).CheckEmailUniqueAsync(Arg.Any<string>());
        await _patientRepository.DidNotReceive().UpdateAsync(Arg.Any<Patient>());
    }
}
