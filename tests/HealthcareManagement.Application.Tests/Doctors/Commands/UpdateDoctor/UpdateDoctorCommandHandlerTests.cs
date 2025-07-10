using HealthcareManagement.Application.Doctors.Commands.UpdateDoctor;
using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Exceptions.Doctor;
using HealthcareManagement.Domain.Exceptions.Shared;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Domain.ValueObjects;
using MediatR;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace HealthcareManagement.Application.Tests.Doctors.Commands.UpdateDoctor;

public class UpdateDoctorCommandHandlerTests
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IDoctorValidationService _doctorValidationService;
    private readonly UpdateDoctorCommandHandler _handler;
    private readonly PersonInfo _initialPersonInfo = PersonInfo.Create(
        FullName.Create("John", "Doe"),
        BirthDate.Create(new DateOnly(1990, 03, 01)),
        Cpf.Create("12345678909"),
        Email.Create("john_doe@email.com"),
        MobilePhoneNumber.Create("11987654321"));

    public UpdateDoctorCommandHandlerTests()
    {
        _doctorValidationService = Substitute.For<IDoctorValidationService>();
        _doctorRepository = Substitute.For<IDoctorRepository>();
        _handler = new UpdateDoctorCommandHandler(_doctorRepository, _doctorValidationService);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesDoctorAndReturnsUnit()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var existingDoctor = new Doctor(doctorId, _initialPersonInfo);
        _doctorValidationService.GetDoctorOrThrowAsync(doctorId).Returns(existingDoctor);
        _doctorValidationService.CheckCpfUniqueAsync(Arg.Any<string>()).Returns(Task.CompletedTask);
        _doctorValidationService.CheckEmailUniqueAsync(Arg.Any<string>()).Returns(Task.CompletedTask);

        Doctor updatedDoctor = null;
        _doctorRepository.UpdateAsync(Arg.Do<Doctor>(d => updatedDoctor = d)).Returns(Task.CompletedTask);

        var command = new UpdateDoctorCommand(doctorId, "John", "Doe", new DateOnly(1988, 5, 15), "12345678910", "11912345678", "doe_john@email.com");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Unit.Value, result);

        await _doctorValidationService.Received(1).GetDoctorOrThrowAsync(doctorId);
        await _doctorValidationService.Received(1).CheckCpfUniqueAsync(command.Cpf);
        await _doctorValidationService.Received(1).CheckEmailUniqueAsync(command.Email);
        await _doctorRepository.Received(1).UpdateAsync(existingDoctor);

        Assert.Equal(command.FirstName, updatedDoctor.PersonInfo.FullName.FirstName);
        Assert.Equal(command.LastName, updatedDoctor.PersonInfo.FullName.LastName);
        Assert.Equal(command.Birthdate, updatedDoctor.PersonInfo.BirthDate.Date);
        Assert.Equal(command.Cpf, updatedDoctor.PersonInfo.Cpf.Number);
        Assert.Equal(command.Email, updatedDoctor.PersonInfo.Email.Adress);
        Assert.Equal(command.PhoneNumber, updatedDoctor.PersonInfo.MobilePhoneNumber.Number);
    }

    [Fact]
    public async Task Handle_DoctorNotFound_ThrowsDoctorNotFoundException_AndDoesNotCallUpdate()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        _doctorValidationService.GetDoctorOrThrowAsync(doctorId)
            .Returns(Task.FromException<Doctor>(new DoctorNotFoundException($"Doctor with ID {doctorId} not found.")));

        var command = new UpdateDoctorCommand(doctorId, "John", "Doe", new DateOnly(1988, 5, 15), "12345678910", "11912345678", "doe_john@email.com");

        // Act & Assert
        await Assert.ThrowsAsync<DoctorNotFoundException>(() => _handler.Handle(command, CancellationToken.None));

        await _doctorValidationService.Received(1).GetDoctorOrThrowAsync(doctorId);
        await _doctorValidationService.DidNotReceive().CheckCpfUniqueAsync(Arg.Any<string>());
        await _doctorValidationService.DidNotReceive().CheckEmailUniqueAsync(Arg.Any<string>());
        await _doctorRepository.DidNotReceive().UpdateAsync(Arg.Any<Doctor>());
    }

    [Fact]
    public async Task Handle_CpfNotUnique_ThrowsCpfNotUniqueException_AndDoesNotCallUpdate()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var existingDoctor = new Doctor(doctorId, _initialPersonInfo);
        _doctorValidationService.GetDoctorOrThrowAsync(doctorId).Returns(existingDoctor);

        _doctorValidationService.CheckCpfUniqueAsync(Arg.Any<string>())
            .ThrowsAsync(new CpfNotUniqueException("Cpf must be unique."));

        var command = new UpdateDoctorCommand(doctorId, "John", "Doe", new DateOnly(1988, 5, 15), "12345678910", "11912345678", "doe_john@email.com");

        // Act & Assert
        await Assert.ThrowsAsync<CpfNotUniqueException>(() => _handler.Handle(command, CancellationToken.None));

        await _doctorValidationService.Received(1).GetDoctorOrThrowAsync(doctorId);
        await _doctorValidationService.Received(1).CheckCpfUniqueAsync(command.Cpf);
        await _doctorValidationService.DidNotReceive().CheckEmailUniqueAsync(Arg.Any<string>());
        await _doctorRepository.DidNotReceive().UpdateAsync(Arg.Any<Doctor>());
    }

    [Fact]
    public async Task Handle_EmailNotUnique_ThrowsEmailNotUniqueException_AndDoesNotCallUpdate()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var existingDoctor = new Doctor(doctorId, _initialPersonInfo);

        _doctorValidationService.GetDoctorOrThrowAsync(doctorId).Returns(existingDoctor);
        _doctorValidationService.CheckCpfUniqueAsync(Arg.Any<string>()).Returns(Task.CompletedTask);
        _doctorValidationService.CheckEmailUniqueAsync(Arg.Any<string>())
            .ThrowsAsync(new EmailNotUniqueException("Email must be unique."));

        var command = new UpdateDoctorCommand(doctorId, "John", "Doe", new DateOnly(1988, 5, 15), "12345678910", "11912345678", "doe_john@email.com");

        // Act & Assert
        await Assert.ThrowsAsync<EmailNotUniqueException>(() => _handler.Handle(command, CancellationToken.None));

        await _doctorValidationService.Received(1).GetDoctorOrThrowAsync(doctorId);
        await _doctorValidationService.Received(1).CheckCpfUniqueAsync(command.Cpf);
        await _doctorValidationService.Received(1).CheckEmailUniqueAsync(Arg.Any<string>());
        await _doctorRepository.DidNotReceive().UpdateAsync(Arg.Any<Doctor>());
    }
}
