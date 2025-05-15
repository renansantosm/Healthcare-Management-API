using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Enums;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Domain.Validation;
using HealthcareManagement.Domain.ValueObjects;
using NSubstitute;

namespace HealthcareManagement.Domain.Tests.Entities;

public class AppointmentTests
{
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();

    private static readonly Guid _guid = Guid.Parse("d2719f5a-3c40-4c2e-9c75-9d9a3b6bfb1d");
    private static readonly Guid _doctorId = Guid.Parse("d2718f5a-2c39-4c2e-9c75-9d9a3b6bfb1d");
    private static readonly Guid _patientId = Guid.Parse("d2912f7a-3c60-7c4e-8c83-9d1a4b6bfb1e");

    [Fact]
    public void CreateAppointment_ValidData_ShouldCreateAppointment()
    {
        // Arrange
        _dateTimeProvider.GetUtcNow().Returns(new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero));

        var appointmentDate = AppointmentDate.Create(new DateTime(2025, 1, 2, 9, 0, 0), _dateTimeProvider);
        var appointment = new Appointment(_guid, _doctorId, _patientId, appointmentDate);

        // Act
        var result = appointment.AppointmentDate;

        // Assert
        Assert.Equal(appointmentDate, result);
        Assert.Equal(_doctorId, appointment.DoctorId);
        Assert.Equal(_patientId, appointment.PatientId);
        Assert.Equal(EAppointmentStatus.Scheduled, appointment.Status);
    }

    [Fact]
    public void CreateAppointment_WithEmptyId_ShouldThrowDomainException()
    {
        // Arrange
        _dateTimeProvider.GetUtcNow().Returns(new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero));

        var appointmentDate = AppointmentDate.Create(new DateTimeOffset(2025, 1, 2, 9, 0, 0, TimeSpan.Zero), _dateTimeProvider);

        // Act & Assert
        var exception = Assert.Throws<DomainValidationException>(() => new Appointment(Guid.Empty, _doctorId, _patientId, appointmentDate));
        Assert.Equal("Id cannot be empty", exception.Message);
    }

    [Fact]
    public void Cancel_ShouldSetStatusToCancelled_WhenMoreThan24HoursBeforeAppointment()
    {
        // Arrange
        _dateTimeProvider.GetUtcNow()
            .Returns(
                new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2025, 1, 2, 8, 0, 0, TimeSpan.Zero) 
            );

        var appointmentDate = AppointmentDate.Create(new DateTimeOffset(2025, 1, 3, 10, 0, 0, TimeSpan.Zero), _dateTimeProvider);
        var appointment = new Appointment(_guid, _doctorId, _patientId, appointmentDate);

        // Act
        appointment.Cancel(_dateTimeProvider);

        // Assert
        Assert.Equal(EAppointmentStatus.Cancelled, appointment.Status);
    }
    
    [Fact]
    public void Cancel_ShouldThrowException_WhenLessThan24HoursBeforeAppointment()
    {
        // Arrange
        _dateTimeProvider.GetUtcNow()
            .Returns(
                new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2025, 1, 2, 10, 0, 0, TimeSpan.Zero)
            );

        var appointmentDate = AppointmentDate.Create(new DateTimeOffset(2025, 1, 3, 9, 0, 0, TimeSpan.Zero), _dateTimeProvider);
        var appointment = new Appointment(_guid, _doctorId, _patientId, appointmentDate);

        // Act & Assert
        var exception = Assert.Throws<DomainValidationException>(() => appointment.Cancel(_dateTimeProvider));
        Assert.Equal("The appointment cannot be canceled within 24 hours of the appointment time", exception.Message);
    }

    [Fact]
    public void CancelAppointment_WithStatusCancelled_ShouldThrowDomainException()
    {
        // Arrange
        _dateTimeProvider.GetUtcNow()
            .Returns(
                new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2025, 1, 2, 10, 0, 0, TimeSpan.Zero)
            );

        var appointmentDate = AppointmentDate.Create(new DateTimeOffset(2025, 1, 3, 11, 0, 0, TimeSpan.Zero), _dateTimeProvider);
        var appointment = new Appointment(_guid, _doctorId, _patientId, appointmentDate);
        appointment.Cancel(_dateTimeProvider);

        // Act & Assert
        var exception = Assert.Throws<DomainValidationException>(() => appointment.Cancel(_dateTimeProvider));
        Assert.Equal("Only scheduled appointments can be canceled", exception.Message);
    }

    [Fact]
    public void CancelAppointment_AppointmentAlreadyStarted_ShouldThrowDomainException()
    {
        // Arrange
        _dateTimeProvider.GetUtcNow()
            .Returns(
                new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2025, 1, 2, 10, 0, 0, TimeSpan.Zero)
            );
        var appointmentDate = AppointmentDate.Create(new DateTimeOffset(2025, 1, 2, 9, 0, 0, TimeSpan.Zero), _dateTimeProvider);
        var appointment = new Appointment(_guid, _doctorId, _patientId, appointmentDate);

        // Act & Assert
        var exception = Assert.Throws<DomainValidationException>(() => appointment.Cancel(_dateTimeProvider));
        Assert.Equal("Cannot cancel an appointment that has already started or finished.", exception.Message);
    }

    [Fact]
    public void RescheduleAppointment_ValidDate_ShouldUpdateAppointmentDate()
    {
        // Arrange
        _dateTimeProvider.GetUtcNow().Returns(new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero));

        var appointmentDate = AppointmentDate.Create(new DateTimeOffset(2025, 1, 2, 9, 0, 0, TimeSpan.Zero), _dateTimeProvider);
        var appointment = new Appointment(_guid, _doctorId, _patientId, appointmentDate);

        var newAppointmentDate = AppointmentDate.Create(new DateTimeOffset(2025, 1, 3, 10, 0, 0, TimeSpan.Zero), _dateTimeProvider);

        // Act
        appointment.Reschedule(newAppointmentDate);

        // Assert
        Assert.Equal(newAppointmentDate, appointment.AppointmentDate);
    }

    [Fact]
    public void RescheduleAppointment_WithStatusCancelled_ShoulThrowException()
    {
        // Arrange
        _dateTimeProvider.GetUtcNow()
            .Returns(
                new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2025, 1, 2, 9, 0, 0, TimeSpan.Zero)
            );

        var appointmentDate = AppointmentDate.Create(new DateTimeOffset(2025, 1, 3, 10, 0, 0, TimeSpan.Zero), _dateTimeProvider);
        var appointment = new Appointment(_guid, _doctorId, _patientId, appointmentDate);
        appointment.Cancel(_dateTimeProvider);

        var newAppointmentDate = AppointmentDate.Create(new DateTimeOffset(2025, 1, 3, 16, 0, 0, TimeSpan.Zero), _dateTimeProvider);

        // Act & Assert
        var exception = Assert.Throws<DomainValidationException>(() => appointment.Reschedule(newAppointmentDate));
        Assert.Equal("Only scheduled appointments can be reschedule", exception.Message);
    }

    [Fact]
    public void CompleteAppointment_ValidDate_ShouldSetStatusToCompleted()
    {
        // Arrange
        _dateTimeProvider.GetUtcNow()
            .Returns(
                new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2025, 1, 2, 10, 0, 0, TimeSpan.Zero)
            );

        var appointmentDate = AppointmentDate.Create(new DateTimeOffset(2025, 1, 2, 9, 0, 0, TimeSpan.Zero), _dateTimeProvider);
        var appointment = new Appointment(_guid, _doctorId, _patientId, appointmentDate);

        // Act
        appointment.Complete(_dateTimeProvider);

        // Assert
        Assert.Equal(EAppointmentStatus.Completed, appointment.Status);
    }

    [Fact]
    public void Complete_ShouldThrowException_WhenAppointmentIsNotScheduled()
    {
        // Arrange
        _dateTimeProvider.GetUtcNow()
            .Returns(
                new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2025, 1, 2, 9, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2025, 1, 3, 11, 0, 0 , TimeSpan.Zero)
            );

        var appointmentDate = AppointmentDate.Create(new DateTimeOffset(2025, 1, 3, 10, 0, 0, TimeSpan.Zero), _dateTimeProvider);
        var appointment = new Appointment(_guid, _doctorId, _patientId, appointmentDate);
        appointment.Cancel(_dateTimeProvider);

        // Act & Assert
        var exception = Assert.Throws<DomainValidationException>(() => appointment.Complete(_dateTimeProvider));
        Assert.Equal("Only scheduled appointments can be completed", exception.Message);
    }

    [Fact]
    public void Complete_ShouldThrowException_WhenAppointmentNotStarted()
    {
        // Arrange
        _dateTimeProvider.GetUtcNow()
            .Returns(
                new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2025, 1, 2, 9, 0, 0, TimeSpan.Zero)
            );

        var appointmentDate = AppointmentDate.Create(new DateTimeOffset(2025, 1, 3, 10, 0, 0, TimeSpan.Zero), _dateTimeProvider);
        var appointment = new Appointment(_guid, _doctorId, _patientId, appointmentDate);

        // Act & Assert
        var exception = Assert.Throws<DomainValidationException>(() => appointment.Complete(_dateTimeProvider));
        Assert.Equal("Cannot complete an appointment that has not started yet", exception.Message);
    }

    [Fact]
    public void Complete_ShoulThrowException_WhenAppointmentNotFinished()
    {
        // Arrange
        _dateTimeProvider.GetUtcNow()
            .Returns(
                new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2025, 1, 3, 10, 15, 0, TimeSpan.Zero)
            );

        var appointmentDate = AppointmentDate.Create(new DateTimeOffset(2025, 1, 3, 10, 0, 0, TimeSpan.Zero), _dateTimeProvider);
        var appointment = new Appointment(_guid, _doctorId, _patientId, appointmentDate);

        // Act & Assert
        var exception = Assert.Throws<DomainValidationException>(() => appointment.Complete(_dateTimeProvider));
        Assert.Equal("Cannot complete an appointment before it ends.", exception.Message);
    }

    [Fact]
    public void AddPrescription_ShouldAddPrescriptionToAppointment()
    {
        // Arrange
        _dateTimeProvider.GetUtcNow().Returns(new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero));

        var appointmentDate = AppointmentDate.Create(new DateTimeOffset(2025, 1, 2, 9, 0, 0, TimeSpan.Zero), _dateTimeProvider);
        var appointment = new Appointment(_guid, _doctorId, _patientId, appointmentDate);

        var prescription = new Prescription(Guid.NewGuid(), appointment.Id, "Medication", "Dosage", "Duration", "Instructions");

        // Act
        appointment.AddPrescription(prescription);

        // Assert
        Assert.Contains(prescription, appointment.Prescriptions);
    }

    [Fact]
    public void AddPrescription_ShouldThrowException_WhenAppointmentIsCancelled()
    {
        // Arrange
        _dateTimeProvider.GetUtcNow().Returns(new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero));

        var appointmentDate = AppointmentDate.Create(new DateTimeOffset(2025, 1, 2, 9, 0, 0, TimeSpan.Zero), _dateTimeProvider);
        var appointment = new Appointment(_guid, _doctorId, _patientId, appointmentDate);

        var prescription = new Prescription(Guid.NewGuid(), appointment.Id, "Medication", "Dosage", "Duration", "Instructions");

        appointment.Cancel(_dateTimeProvider);

        // Act & Assert
        var exception = Assert.Throws<DomainValidationException>(() => appointment.AddPrescription(prescription));
        Assert.Equal("Only scheduled appointments can have prescriptions added", exception.Message);
    }

    [Fact]
    public void AddPrescription_ShouldThrowException_WhenAppointmentIsCompleted()
    {
        // Arrange
        _dateTimeProvider.GetUtcNow()
             .Returns(
                 new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero),
                 new DateTimeOffset(2025, 1, 2, 10, 0, 0, TimeSpan.Zero)
             );

        var appointmentDate = AppointmentDate.Create(new DateTimeOffset(2025, 1, 2, 9, 0, 0, TimeSpan.Zero), _dateTimeProvider);
        var appointment = new Appointment(_guid, _doctorId, _patientId, appointmentDate);
        var prescription = new Prescription(Guid.NewGuid(), appointment.Id, "Medication", "Dosage", "Duration", "Instructions");

        appointment.Complete(_dateTimeProvider);

        // Act & Assert
        var exception = Assert.Throws<DomainValidationException>(() => appointment.AddPrescription(prescription));
        Assert.Equal("Only scheduled appointments can have prescriptions added", exception.Message);
    }

    [Fact]
    public void UpdatePrescription_ValidData_ShouldUpdatePrescription()
    {
        // Arrange
        _dateTimeProvider.GetUtcNow().Returns(new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero));

        var appointmentDate = AppointmentDate.Create(new DateTimeOffset(2025, 1, 2, 9, 0, 0, TimeSpan.Zero), _dateTimeProvider);
        var appointment = new Appointment(_guid, _doctorId, _patientId, appointmentDate);
        var prescription = new Prescription(Guid.NewGuid(), appointment.Id, "Medication", "Dosage", "Duration", "Instructions");

        appointment.AddPrescription(prescription);

        // Act
        appointment.UpdatePrescription(prescription.Id, "New Medication", "New Dosage", "New Duration", "New Instructions");

        // Assert
        Assert.Equal(prescription.AppointmentId, appointment.Id);
        Assert.Equal("New Medication", prescription.Medication);
        Assert.Equal("New Dosage", prescription.Dosage);
        Assert.Equal("New Duration", prescription.Duration);
        Assert.Equal("New Instructions", prescription.Instructions);
    }

    [Fact]
    public void UpdatePrescription_ShouldThrowException_WhenAppointmentIsCancelled()
    {
        // Arrange
        _dateTimeProvider.GetUtcNow().Returns(new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero));

        var appointmentDate = AppointmentDate.Create(new DateTimeOffset(2025, 1, 2, 9, 0, 0, TimeSpan.Zero), _dateTimeProvider);
        var appointment = new Appointment(_guid, _doctorId, _patientId, appointmentDate);
        var prescription = new Prescription(Guid.NewGuid(), appointment.Id, "Medication", "Dosage", "Duration", "Instructions");

        appointment.AddPrescription(prescription);

        appointment.Cancel(_dateTimeProvider);

        // Act & Assert
        var exception = Assert.Throws<DomainValidationException>(() => appointment.UpdatePrescription(prescription.Id, "New Medication", "New Dosage", "New Duration", "New Instructions"));
        Assert.Equal("Only scheduled appointments can have prescriptions added", exception.Message);
    }

    [Fact]
    public void UpdatePrescription_ShouldThrowException_WhenAppointmentIsCompleted()
    {
        // Arrange
        _dateTimeProvider.GetUtcNow()
            .Returns(
                new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2025, 1, 2, 10, 0, 0, TimeSpan.Zero)
            );

        var appointmentDate = AppointmentDate.Create(new DateTimeOffset(2025, 1, 2, 9, 0, 0, TimeSpan.Zero), _dateTimeProvider);
        var appointment = new Appointment(_guid, _doctorId, _patientId, appointmentDate);
        var prescription = new Prescription(Guid.NewGuid(), appointment.Id, "Medication", "Dosage", "Duration", "Instructions");

        appointment.AddPrescription(prescription);

        appointment.Complete(_dateTimeProvider);

        // Act & Assert
        var exception = Assert.Throws<DomainValidationException>(() => appointment.UpdatePrescription(prescription.Id, "New Medication", "New Dosage", "New Duration", "New Instructions"));
        Assert.Equal("Only scheduled appointments can have prescriptions added", exception.Message);
    }
}
