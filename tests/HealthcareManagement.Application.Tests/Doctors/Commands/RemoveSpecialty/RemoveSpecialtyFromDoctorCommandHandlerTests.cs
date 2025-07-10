using HealthcareManagement.Application.Doctors.Commands.RemoveSpecialty;
using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Exceptions.Doctor;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Domain.ValueObjects;
using MediatR;
using NSubstitute;

namespace HealthcareManagement.Application.Tests.Doctors.Commands.RemoveSpecialty;

public class RemoveSpecialtyFromDoctorCommandHandlerTests
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IDoctorValidationService _doctorValidationService;
    private readonly RemoveSpecialtyFromDoctorCommandHandler _handler;
    private readonly PersonInfo _personInfo = PersonInfo.Create(
        FullName.Create("John", "Doe"),
        BirthDate.Create(new DateOnly(1990, 03, 01)),
        Cpf.Create("12345678909"),
        Email.Create("john_doe@email.com"),
        MobilePhoneNumber.Create("11987654321"));

    public RemoveSpecialtyFromDoctorCommandHandlerTests()
    {
        _doctorRepository = Substitute.For<IDoctorRepository>();
        _doctorValidationService = Substitute.For<IDoctorValidationService>();
        _handler = new RemoveSpecialtyFromDoctorCommandHandler(_doctorRepository, _doctorValidationService);
    }

    [Fact]
    public async Task Handle_ValidCommand_RemovesSpecialtyAndUpdatesRepository()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var doctor = new Doctor(doctorId, _personInfo);
        doctor.AddSpecialty(Specialty.Create("Cardiology"));
        doctor.AddSpecialty(Specialty.Create("Dermatology"));

        _doctorValidationService.GetDoctorOrThrowAsync(doctorId).Returns(Task.FromResult(doctor));

        Doctor capturedDoctor = null;
        _doctorRepository.UpdateAsync(Arg.Do<Doctor>(Doctor => capturedDoctor = doctor)).Returns(Task.CompletedTask);

        var command = new RemoveSpecialtyFromDoctorCommand(doctorId, "Cardiology");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Unit.Value, result);
        await _doctorValidationService.Received(1).GetDoctorOrThrowAsync(doctorId);
        await _doctorRepository.Received(1).UpdateAsync(doctor);

        Assert.DoesNotContain(capturedDoctor.Specialties, s => s.Name == "cardiology");
        Assert.Contains(capturedDoctor.Specialties, s => s.Name == "dermatology");
    }

    [Fact]
    public async Task Handle_DoctorNotFound_ThrowsDoctorNotFoundException_AndDoesNotCallUpdate()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        _doctorValidationService.GetDoctorOrThrowAsync(doctorId)
            .Returns(Task.FromException<Doctor>(new DoctorNotFoundException($"Doctor with ID {doctorId} not found.")));

        var command = new RemoveSpecialtyFromDoctorCommand(doctorId, "Cardiology");

        // Act & Assert
        await Assert.ThrowsAsync<DoctorNotFoundException>(() => _handler.Handle(command, CancellationToken.None));

        await _doctorValidationService.Received(1).GetDoctorOrThrowAsync(doctorId);
        await _doctorRepository.DidNotReceive().UpdateAsync(Arg.Any<Doctor>());
    }
}
