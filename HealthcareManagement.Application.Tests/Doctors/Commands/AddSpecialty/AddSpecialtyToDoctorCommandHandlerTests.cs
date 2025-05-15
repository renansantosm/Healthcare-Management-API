using HealthcareManagement.Application.Doctors.Commands.AddSpecialty;
using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Exceptions.Doctor;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Domain.ValueObjects;
using MediatR;
using NSubstitute;

namespace HealthcareManagement.Application.Tests.Doctors.Commands.AddSpecialty;

public class AddSpecialtyToDoctorCommandHandlerTests
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IDoctorValidationService _doctorValidationService;
    private readonly AddSpecialtyToDoctorCommandHandler _handler;
    private readonly PersonInfo _personInfo = PersonInfo.Create(
        FullName.Create("John", "Doe"),
        BirthDate.Create(new DateOnly(1990, 03, 01)),
        Cpf.Create("12345678909"),
        Email.Create("john_doe@email.com"),
        MobilePhoneNumber.Create("11987654321"));

    public AddSpecialtyToDoctorCommandHandlerTests()
    {
        _doctorRepository = Substitute.For<IDoctorRepository>();
        _doctorValidationService = Substitute.For<IDoctorValidationService>();
        _handler = new AddSpecialtyToDoctorCommandHandler(_doctorRepository, _doctorValidationService);
    }

    [Fact]
    public async Task Handle_ValidCommand_AddsSpecialtyAndUpdatesRepository()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var existingDoctor = new Doctor(doctorId, _personInfo);
        _doctorValidationService.GetDoctorOrThrowAsync(doctorId).Returns(existingDoctor);

        Doctor capturedDoctor = null;
        _doctorRepository.UpdateAsync(Arg.Do<Doctor>(d => capturedDoctor = d)).Returns(Task.CompletedTask);

        var command = new AddSpecialtyToDoctorCommand(doctorId, "Cardiology");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Unit.Value, result);

        await _doctorValidationService.Received(1).GetDoctorOrThrowAsync(doctorId);
        await _doctorRepository.Received(1).UpdateAsync(existingDoctor);

        Assert.NotNull(capturedDoctor);
        var exceptedSpecialty = Specialty.Create("Cardiology");
        Assert.Contains(exceptedSpecialty, capturedDoctor.Specialties);
    }

    [Fact]
    public async Task Handle_DoctorNotFound_ThrowsDoctorNotFoundException_AndDoesNotCallUpdate()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        _doctorValidationService.GetDoctorOrThrowAsync(doctorId)
            .Returns(Task.FromException<Doctor>(new DoctorNotFoundException($"Doctor with ID {doctorId} not found.")));

        var command = new AddSpecialtyToDoctorCommand(doctorId, "Cardiology");

        // Act & Assert
        await Assert.ThrowsAsync<DoctorNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        await _doctorValidationService.Received(1).GetDoctorOrThrowAsync(doctorId);
        await _doctorRepository.DidNotReceive().UpdateAsync(Arg.Any<Doctor>());
    }
}
