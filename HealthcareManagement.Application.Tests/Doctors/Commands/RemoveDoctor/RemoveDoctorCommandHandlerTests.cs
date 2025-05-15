using HealthcareManagement.Application.Doctors.Commands.RemoveDoctor;
using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Exceptions.Doctor;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Domain.ValueObjects;
using MediatR;
using NSubstitute;

namespace HealthcareManagement.Application.Tests.Doctors.Commands.RemoveDoctor;

public class RemoveDoctorCommandHandlerTests
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IDoctorValidationService _doctorValidationService;
    private readonly RemoveDoctorCommandHandler _handler;
    private readonly PersonInfo _personInfo = PersonInfo.Create(
        FullName.Create("John", "Doe"), 
        BirthDate.Create(new DateOnly(1990, 03, 01)),
        Cpf.Create("12345678909"),
        Email.Create("john_doe@email.com"),
        MobilePhoneNumber.Create("11987654321"));

    public RemoveDoctorCommandHandlerTests()
    {
        _doctorRepository = Substitute.For<IDoctorRepository>();
        _doctorValidationService = Substitute.For<IDoctorValidationService>();
        _handler = new RemoveDoctorCommandHandler(_doctorRepository, _doctorValidationService);
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsValidationsAndDeletesDoctor()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var command = new RemoveDoctorCommand(doctorId);
        var doctor = new Doctor(doctorId, _personInfo);
        _doctorValidationService.GetDoctorOrThrowAsync(doctorId).Returns(Task.FromResult(doctor));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Unit.Value, result);

        await _doctorValidationService.Received(1).GetDoctorOrThrowAsync(doctorId);
        await _doctorRepository.Received(1).DeleteAsync(doctor);
    }

    [Fact]
    public async Task Handle_DoctorDoesNotExist_ThrowsDoctorNotFoundException_AndDoesNotCallDelete()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var command = new RemoveDoctorCommand(doctorId);
        _doctorValidationService.GetDoctorOrThrowAsync(doctorId).Returns(Task.FromException<Doctor>(new DoctorNotFoundException($"Doctor with ID {doctorId} not found.")));

        // Act & Assert
        await Assert.ThrowsAsync<DoctorNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        await _doctorValidationService.Received(1).GetDoctorOrThrowAsync(doctorId);
        await _doctorRepository.DidNotReceive().DeleteAsync(Arg.Any<Doctor>());
    }
}
