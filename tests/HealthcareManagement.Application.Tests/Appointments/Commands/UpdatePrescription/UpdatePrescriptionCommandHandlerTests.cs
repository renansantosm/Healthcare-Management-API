using HealthcareManagement.Application.Appointments.Commands.UpdatePrescription;
using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Exceptions.Appointment;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Domain.Validation;
using HealthcareManagement.Domain.ValueObjects;
using MediatR;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace HealthcareManagement.Application.Tests.Appointments.Commands.UpdatePrescription;

public class UpdatePrescriptionCommandHandlerTests
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentValidationService _appointmentValidationService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly UpdatePrescriptionCommandHandler _handler;

    public UpdatePrescriptionCommandHandlerTests()
    {
        _appointmentRepository = Substitute.For<IAppointmentRepository>();
        _appointmentValidationService = Substitute.For<IAppointmentValidationService>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        _handler = new UpdatePrescriptionCommandHandler(_appointmentRepository,_appointmentValidationService);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesExistingPrescriptionAndReturnsUnit()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var prescriptionId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var patientId = Guid.NewGuid();

        _dateTimeProvider.GetUtcNow().Returns(new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero));

        var appointmentDate = AppointmentDate.Create(new DateTimeOffset(2025, 1, 2, 9, 0, 0, TimeSpan.Zero), _dateTimeProvider);
        var appointment = new Appointment(appointmentId, doctorId, patientId, appointmentDate);

        var existingPrescription = new Prescription(prescriptionId, appointmentId, "Dipirona", "10 gotas", "5 dias", "Tomar após as refeições");

        appointment.AddPrescription(existingPrescription);

        _appointmentValidationService.GetAppointmentWithPrescriptionsOrThrowAsync(appointmentId).Returns(Task.FromResult(appointment));

        Appointment capturedAppointment = null;
        _appointmentRepository.UpdateAsync(Arg.Do<Appointment>(a => capturedAppointment = a)).Returns(Task.CompletedTask);

        var command = new UpdatePrescriptionCommand(appointmentId, prescriptionId, "Dipirona", "20 gotas", "7 dias", "Tomar antes das refeições");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Unit.Value, result);

        await _appointmentValidationService.Received(1).GetAppointmentWithPrescriptionsOrThrowAsync(appointmentId);
        await _appointmentRepository.Received(1).UpdateAsync(Arg.Any<Appointment>());

        var prescription = capturedAppointment.Prescriptions.FirstOrDefault(p => p.Id == prescriptionId);

        Assert.Equal(appointmentId, prescription.AppointmentId);
        Assert.Equal(command.Medication, prescription.Medication);
        Assert.Equal(command.Dosage, prescription.Dosage);
        Assert.Equal(command.Duration, prescription.Duration);
        Assert.Equal(command.Instructions, prescription.Instructions);
        Assert.NotEqual(Guid.Empty, prescription.Id);
    }

    [Fact]
    public async Task Handle_AppointmentNotFound_ThrowsAndDoesNotCallUpdate() 
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var prescriptionId = Guid.NewGuid();

        _appointmentValidationService.GetAppointmentWithPrescriptionsOrThrowAsync(appointmentId)
            .ThrowsAsync(new AppointmentNotFoundException($"Appointment with ID {appointmentId} not found."));

        var command = new UpdatePrescriptionCommand(appointmentId, prescriptionId, "Dipirona", "10 gotas", "5 dias", "Tomar após as refeições");


        // Act & Assert
        await Assert.ThrowsAsync<AppointmentNotFoundException>(() =>_handler.Handle(command, CancellationToken.None));

        await _appointmentValidationService.Received(1).GetAppointmentWithPrescriptionsOrThrowAsync(appointmentId);
        await _appointmentRepository.DidNotReceive().UpdateAsync(Arg.Any<Appointment>());
    }
}
