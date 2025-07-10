using HealthcareManagement.Application.Appointments.Commands.Complete;
using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Exceptions.Appointment;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Domain.ValueObjects;
using MediatR;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace HealthcareManagement.Application.Tests.Appointments.Commands.Complete;

public class CompleteAppointmentCommandHandlerTests
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IAppointmentValidationService _appointmentValidationService;
    private readonly CompleteAppointmentCommandHandler _handler;

    public CompleteAppointmentCommandHandlerTests()
    {
        _appointmentRepository = Substitute.For<IAppointmentRepository>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        _appointmentValidationService = Substitute.For<IAppointmentValidationService>();
        _handler = new CompleteAppointmentCommandHandler(_appointmentRepository, _appointmentValidationService, _dateTimeProvider);
    }

    [Fact]
    public async Task Handle_ValidCommand_CompletesAppointmentAndUpdatesRepository()
    {
        // Arrange
        _dateTimeProvider.GetUtcNow()
            .Returns(
                new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2025, 1, 2, 10, 0, 0, TimeSpan.Zero)
            );

        var appointmentId = Guid.NewGuid();

        var appointmentDate = AppointmentDate.Create(new DateTimeOffset(2025, 1, 2, 9, 0, 0, TimeSpan.Zero), _dateTimeProvider);

        var appointment = new Appointment(appointmentId, Guid.NewGuid(), Guid.NewGuid(), appointmentDate);

        _appointmentValidationService.GetAppointmentOrThrowAsync(appointmentId).Returns(Task.FromResult(appointment));

        Appointment capturedAppointment = null;
        _appointmentRepository.UpdateAsync(Arg.Do<Appointment>(a => capturedAppointment = a)).Returns(Task.CompletedTask);

        var command = new CompleteAppointmentCommand(appointmentId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Unit.Value, result);
        Assert.NotNull(capturedAppointment);

        await _appointmentValidationService.Received(1).GetAppointmentOrThrowAsync(appointmentId);
        await _appointmentRepository.Received(1).UpdateAsync(appointment);
    }

    [Fact]
    public async Task Handle_AppointmentNotFound_ThrowsAppointmentNotFoundException_AndDoesNotCallUpdate()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var command = new CompleteAppointmentCommand(appointmentId);
        _appointmentValidationService.GetAppointmentOrThrowAsync(appointmentId)
                .ThrowsAsync(new AppointmentNotFoundException($"Appointment with ID {appointmentId} not found"));

        // Act & Assert
        await Assert.ThrowsAsync<AppointmentNotFoundException>(() => _handler.Handle(command, CancellationToken.None));

        await _appointmentValidationService.Received(1).GetAppointmentOrThrowAsync(appointmentId);
        await _appointmentRepository.DidNotReceive().UpdateAsync(Arg.Any<Appointment>());
    }
}
