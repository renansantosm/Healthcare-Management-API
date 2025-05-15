using HealthcareManagement.Application.Appointments.Commands.AddPrescription;
using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Exceptions.Appointment;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Domain.ValueObjects;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace HealthcareManagement.Application.Tests.Appointments.Commands.AddPrescription;

public class AddPrescriptionCommandHandlerTests
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentValidationService _appointmentValidationService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly AddPrescriptionCommandHandler _handler;

    public AddPrescriptionCommandHandlerTests()
    {
        _appointmentRepository = Substitute.For<IAppointmentRepository>();
        _appointmentValidationService = Substitute.For<IAppointmentValidationService>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        _handler = new AddPrescriptionCommandHandler(_appointmentRepository, _appointmentValidationService);
    }

    [Fact]
    public async Task Handle_ValidCommand_AddsPrescriptionAndReturnsId()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var command = new AddPrescriptionCommand(
            appointmentId,
            "Dipirona",
            "20 gotas",
            "7 dias",
            "Tomar antes das refeições");

        Prescription capturedPrescription = null;
        _appointmentRepository
            .When(x => x.AddPrescriptionAsync(Arg.Any<Guid>(), Arg.Any<Prescription>()))
            .Do(info => capturedPrescription = info.Arg<Prescription>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _appointmentValidationService.Received(1).ValidateAppointmentExistsOrThrowAsync(appointmentId);
        await _appointmentRepository.Received(1).AddPrescriptionAsync(appointmentId, Arg.Any<Prescription>());

        Assert.Equal(result, capturedPrescription.Id);
        Assert.Equal(appointmentId, capturedPrescription.AppointmentId);
        Assert.Equal(command.Medication, capturedPrescription.Medication);
        Assert.Equal(command.Dosage, capturedPrescription.Dosage);
        Assert.Equal(command.Duration, capturedPrescription.Duration);
        Assert.Equal(command.Instructions, capturedPrescription.Instructions);
        Assert.NotEqual(Guid.Empty, capturedPrescription.Id);
    }

    [Fact]
    public async Task Handle_WithExistingPrescription_AddsAnotherPrescriptionAndPreservesBoth()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var existingPrescriptionId = Guid.NewGuid();

        _dateTimeProvider.GetUtcNow().Returns(new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero));

        var appointmentDate = AppointmentDate.Create(new DateTimeOffset(2025, 1, 2, 9, 0, 0, TimeSpan.Zero), _dateTimeProvider);
        var appointment = new Appointment(appointmentId, doctorId, patientId, appointmentDate);

        var existingPrescription = new Prescription(existingPrescriptionId, appointmentId, "Ibuprofeno", "1 comprimido", "5 dias", "Tomar após as refeições");

        appointment.AddPrescription(existingPrescription);

        _appointmentValidationService.ValidateAppointmentExistsOrThrowAsync(appointmentId).Returns(Task.CompletedTask);

        _appointmentRepository.AddPrescriptionAsync(Arg.Any<Guid>(), Arg.Any<Prescription>())
            .Returns(callInfo => {
                var prescription = callInfo.Arg<Prescription>();
                return Task.FromResult(prescription.Id);
            });

        Prescription capturedPrescription = null;
        _appointmentRepository
            .When(x => x.AddPrescriptionAsync(Arg.Any<Guid>(), Arg.Any<Prescription>()))
            .Do(info => capturedPrescription = info.Arg<Prescription>());

        var command = new AddPrescriptionCommand(
            appointmentId,
            "Dipirona",
            "20 gotas",
            "7 dias",
            "Tomar antes das refeições");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _appointmentValidationService.Received(1).ValidateAppointmentExistsOrThrowAsync(appointmentId);
        await _appointmentRepository.Received(1).AddPrescriptionAsync(appointmentId, Arg.Any<Prescription>());

        Assert.NotNull(capturedPrescription);
        Assert.Equal(result, capturedPrescription.Id);
        Assert.Equal(appointmentId, capturedPrescription.AppointmentId);
        Assert.Equal("Dipirona", capturedPrescription.Medication);
        Assert.Equal("20 gotas", capturedPrescription.Dosage);
        Assert.Equal("7 dias", capturedPrescription.Duration);
        Assert.Equal("Tomar antes das refeições", capturedPrescription.Instructions);
    }

    [Fact]
    public async Task Handle_AppointmentNotFound_ThrowsAndDoesNotCallUpdate()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var command = new AddPrescriptionCommand(
            appointmentId,
            "Dipirona",
            "20 gotas",
            "7 dias",
            "Tomar antes das refeições");

        _appointmentValidationService.ValidateAppointmentExistsOrThrowAsync(appointmentId)
            .ThrowsAsync(new AppointmentNotFoundException($"Appointment {appointmentId} not found."));

        // Act & Assert
        await Assert.ThrowsAsync<AppointmentNotFoundException>(() => _handler.Handle(command, CancellationToken.None));

        await _appointmentValidationService.Received(1).ValidateAppointmentExistsOrThrowAsync(appointmentId);
        await _appointmentRepository.DidNotReceive().AddPrescriptionAsync(Arg.Any<Guid>(), Arg.Any<Prescription>());
    }
}