using HealthcareManagement.Application.Doctors.Commands.RemoveDoctor;
using HealthcareManagement.Application.Patients.Commands.RemovePatient;
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

namespace HealthcareManagement.Application.Tests.Patients.RemovePatient;

public class RemovePatientCommandHandlerTests
{
    private readonly IPatientRepository _patientRepository;
    private readonly IPatientValidationService _patientValidationService;
    private readonly RemovePatientCommandHandler _handler;
    private readonly PersonInfo _personInfo = PersonInfo.Create(
        FullName.Create("John", "Doe"),
        BirthDate.Create(new DateOnly(1990, 03, 01)),
        Cpf.Create("12345678909"),
        Email.Create("john_doe@email.com"),
        MobilePhoneNumber.Create("11987654321"));

    public RemovePatientCommandHandlerTests()
    {
        _patientRepository = Substitute.For<IPatientRepository>();
        _patientValidationService = Substitute.For<IPatientValidationService>();
        _handler = new RemovePatientCommandHandler(_patientRepository, _patientValidationService);
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsValidationsAndDeletesDoctor()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var command = new RemovePatientCommand(patientId);
        var patient = new Patient(patientId, _personInfo);
        _patientValidationService.GetPatientOrThrowAsync(patientId).Returns(Task.FromResult(patient));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Unit.Value, result);

        await _patientValidationService.Received(1).GetPatientOrThrowAsync(patientId);
        await _patientRepository.Received(1).DeleteAsync(patient);
    }

    [Fact]
    public async Task Handle_PatientDoesNotExist_ThrowsPatientNotFoundException_AndDoesNotCallDelete()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var command = new RemovePatientCommand(patientId);
        _patientValidationService.GetPatientOrThrowAsync(patientId)
            .Returns(Task.FromException<Patient>(new PatientNotFoundException($"Patient with ID {patientId} not found.")));

        // Act & Assert
        await Assert.ThrowsAsync<PatientNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        await _patientValidationService.Received(1).GetPatientOrThrowAsync(patientId);
        await _patientRepository.DidNotReceive().DeleteAsync(Arg.Any<Patient>());
    }
}
