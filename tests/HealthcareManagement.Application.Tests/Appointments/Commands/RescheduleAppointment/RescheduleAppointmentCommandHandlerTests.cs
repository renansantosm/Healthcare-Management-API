using HealthcareManagement.Application.Appointments.Commands.Reschedule;
using HealthcareManagement.Application.Appointments.Commands.RescheduleAppointment;
using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Exceptions.Appointment;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Domain.ValueObjects;
using MediatR;
using NSubstitute;
using System.Text.RegularExpressions;

namespace HealthcareManagement.Application.Tests.Appointments.Commands.Reschedule;

public class RescheduleAppointmentCommandHandlerTests
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IAppointmentValidationService _appointmentValidationService;
    private readonly RescheduleAppointmentCommandHandler _handler;

    public RescheduleAppointmentCommandHandlerTests()
    {
        _appointmentRepository = Substitute.For<IAppointmentRepository>();
        _appointmentValidationService = Substitute.For<IAppointmentValidationService>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        _handler = new RescheduleAppointmentCommandHandler(_appointmentRepository, _dateTimeProvider, _appointmentValidationService);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReschedulesAndUpdatesRepository()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var patientId = Guid.NewGuid();

        _dateTimeProvider.GetUtcNow().Returns(new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero));

        var currentAppointmentDate = AppointmentDate.Create(new DateTimeOffset(2025, 1, 2, 9, 0, 0, TimeSpan.Zero), _dateTimeProvider);

        var appointment = new Appointment(appointmentId, doctorId, patientId, currentAppointmentDate);
        _appointmentValidationService.GetAppointmentOrThrowAsync(appointmentId).Returns(Task.FromResult(appointment));

        var newAppointmentDate = new DateTimeOffset(2025, 1, 3, 10, 0, 0, TimeSpan.Zero);
        _appointmentValidationService.EnsureDoctorHasNoConflictingAppointmentsAsync(doctorId, newAppointmentDate).Returns(Task.CompletedTask);
        _appointmentValidationService.EnsurePatientHasNoConflictingAppointmentsAsync(patientId, newAppointmentDate).Returns(Task.CompletedTask);

        Appointment capturedAppointment = null;
        _appointmentRepository.UpdateAsync(Arg.Do<Appointment>(a => capturedAppointment = a)).Returns(Task.CompletedTask);

        var command = new RescheduleAppointmentCommand(appointmentId, newAppointmentDate);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Unit.Value, result);
        await _appointmentValidationService.Received(1).GetAppointmentOrThrowAsync(appointmentId);
        await _appointmentValidationService.Received(1).EnsureDoctorHasNoConflictingAppointmentsAsync(doctorId, newAppointmentDate);
        await _appointmentValidationService.Received(1).EnsurePatientHasNoConflictingAppointmentsAsync(patientId, newAppointmentDate);
        await _appointmentRepository.Received(1).UpdateAsync(appointment);
        Assert.NotNull(capturedAppointment);
    }

    [Fact]
    public async Task Handle_AppointmentNotFound_ThrowsAndDoesNotCallUpdate()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        _appointmentValidationService.GetAppointmentOrThrowAsync(appointmentId)
            .Returns(Task.FromException<Appointment>(new AppointmentNotFoundException($"Appointment with ID {appointmentId} not found.")));

        var command = new RescheduleAppointmentCommand(appointmentId, DateTimeOffset.UtcNow);

        // Act & Assert
        await Assert.ThrowsAsync<AppointmentNotFoundException>(() => _handler.Handle(command, CancellationToken.None));

        await _appointmentValidationService.Received(1).GetAppointmentOrThrowAsync(appointmentId);
        await _appointmentRepository.DidNotReceive().UpdateAsync(Arg.Any<Appointment>());
        await _appointmentValidationService.DidNotReceive().EnsureDoctorHasNoConflictingAppointmentsAsync(Arg.Any<Guid>(), Arg.Any<DateTimeOffset>());
        await _appointmentValidationService.DidNotReceive().EnsurePatientHasNoConflictingAppointmentsAsync(Arg.Any<Guid>(), Arg.Any<DateTimeOffset>());
    }

    [Fact]
    public async Task Handle_DoctorConflict_ThrowsAndDoesNotCallUpdate()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var patientId = Guid.NewGuid();

        _dateTimeProvider.GetUtcNow().Returns(new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero));

        var currentAppointmentDate = AppointmentDate.Create(new DateTimeOffset(2025, 1, 2, 9, 0, 0, TimeSpan.Zero), _dateTimeProvider);
        var appointment = new Appointment(appointmentId, doctorId, patientId, currentAppointmentDate);
        _appointmentValidationService.GetAppointmentOrThrowAsync(appointmentId).Returns(Task.FromResult(appointment));

        var newAppointmentDate = new DateTimeOffset(2025, 1, 3, 10, 0, 0, TimeSpan.Zero);
        _appointmentValidationService.EnsureDoctorHasNoConflictingAppointmentsAsync(doctorId, newAppointmentDate)
            .Returns(Task.FromException(new AppointmentConflictException($"The doctor with ID '{doctorId}' already has an appointment scheduled at {newAppointmentDate:yyyy-MM-dd HH:mm}.")));

        var command = new RescheduleAppointmentCommand(appointmentId, newAppointmentDate);

        // Act & Assert
        await Assert.ThrowsAsync<AppointmentConflictException>(() => _handler.Handle(command, CancellationToken.None));

        await _appointmentValidationService.Received(1).GetAppointmentOrThrowAsync(appointmentId);
        await _appointmentValidationService.Received(1).EnsureDoctorHasNoConflictingAppointmentsAsync(Arg.Any<Guid>(), Arg.Any<DateTimeOffset>());
        await _appointmentValidationService.DidNotReceive().EnsurePatientHasNoConflictingAppointmentsAsync(Arg.Any<Guid>(), Arg.Any<DateTimeOffset>());
        await _appointmentRepository.DidNotReceive().UpdateAsync(Arg.Any<Appointment>());
    }

    [Fact]
    public async Task Handle_PatientConflict_ThrowsAndDoesNotCallUpdate()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var patientId = Guid.NewGuid();

        _dateTimeProvider.GetUtcNow().Returns(new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero));

        var currentAppointmentDate = AppointmentDate.Create(new DateTimeOffset(2025, 1, 2, 9, 0, 0, TimeSpan.Zero), _dateTimeProvider);
        var appointment = new Appointment(appointmentId, doctorId, patientId, currentAppointmentDate);
        _appointmentValidationService.GetAppointmentOrThrowAsync(appointmentId).Returns(Task.FromResult(appointment));


        var newAppointmentDate = new DateTimeOffset(2025, 1, 3, 10, 0, 0, TimeSpan.Zero);
        _appointmentValidationService.EnsureDoctorHasNoConflictingAppointmentsAsync(doctorId, newAppointmentDate).Returns(Task.CompletedTask);

        _appointmentValidationService.EnsurePatientHasNoConflictingAppointmentsAsync(patientId, newAppointmentDate)
            .Returns(Task.FromException(new AppointmentConflictException($"The patient with ID '{patientId}' already has an appointment scheduled at {newAppointmentDate:yyyy-MM-dd HH:mm}.")));

        var command = new RescheduleAppointmentCommand(appointmentId, newAppointmentDate);

        // Act & Assert
        await Assert.ThrowsAsync<AppointmentConflictException>(() =>_handler.Handle(command, CancellationToken.None));

        await _appointmentValidationService.Received(1).GetAppointmentOrThrowAsync(appointmentId);
        await _appointmentValidationService.Received(1).EnsureDoctorHasNoConflictingAppointmentsAsync(Arg.Any<Guid>(), Arg.Any<DateTimeOffset>());
        await _appointmentValidationService.Received(1).EnsurePatientHasNoConflictingAppointmentsAsync(Arg.Any<Guid>(), Arg.Any<DateTimeOffset>());
        await _appointmentRepository.DidNotReceive().UpdateAsync(Arg.Any<Appointment>());
    }
}