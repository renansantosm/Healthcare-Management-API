using HealthcareManagement.Application.Appointments.Commands.CancelAppointment;
using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Enums;
using HealthcareManagement.Domain.Exceptions.Appointment;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Domain.ValueObjects;
using MediatR;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace HealthcareManagement.Application.Tests.Appointments.Commands.Cancel;

public class CancelAppointmentCommandHandlerTests
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentValidationService _appointmentValidationService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly CancelAppointmentCommandHandler _handler;

    public CancelAppointmentCommandHandlerTests()
    {
        _appointmentRepository = Substitute.For<IAppointmentRepository>();
        _appointmentValidationService = Substitute.For<IAppointmentValidationService>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();

        _handler = new CancelAppointmentCommandHandler(_appointmentRepository,_appointmentValidationService,_dateTimeProvider);
    }

    [Fact]
    public async Task Handle_ValidCommand_CancelsAppointmentAndUpdatesRepository()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var patientId = Guid.NewGuid();

        _dateTimeProvider.GetUtcNow().Returns(new DateTimeOffset(2025, 5, 1, 8, 0, 0, TimeSpan.Zero));

        var appointmentDate = AppointmentDate.Create(
                new DateTimeOffset(2025, 05, 07, 16, 00, 00, TimeSpan.Zero),
                _dateTimeProvider
            );

        var existingAppointment = new Appointment(appointmentId,doctorId, patientId, appointmentDate);
        _appointmentValidationService.GetAppointmentOrThrowAsync(appointmentId).Returns(Task.FromResult(existingAppointment));

        Appointment capturedAppointment = null;
        _appointmentRepository.UpdateAsync(Arg.Do<Appointment>(a => capturedAppointment = a)).Returns(Task.CompletedTask);

        var command = new CancelAppointmentCommand(appointmentId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Unit.Value, result);
        Assert.Equal(EAppointmentStatus.Cancelled, capturedAppointment.Status);

        await _appointmentValidationService.Received(1).GetAppointmentOrThrowAsync(appointmentId);
        await _appointmentRepository.Received(1).UpdateAsync(existingAppointment);

        Assert.NotNull(capturedAppointment);
    }

    [Fact]
    public async Task Handle_AppointmentNotFound_ThrowsAndDoesNotCallUpdate()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();

        _appointmentValidationService.GetAppointmentOrThrowAsync(appointmentId)
            .ThrowsAsync(new AppointmentNotFoundException($"Appointment {appointmentId} not found"));

        var command = new CancelAppointmentCommand(appointmentId);

        // Act & Assert
        await Assert.ThrowsAsync<AppointmentNotFoundException>(() => _handler.Handle(command, CancellationToken.None));

        await _appointmentRepository.DidNotReceive().UpdateAsync(Arg.Any<Appointment>());
    }
}
