using HealthcareManagement.Application.Services.Validation;
using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Exceptions.Appointment;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Domain.ValueObjects;
using NSubstitute;

namespace HealthcareManagement.Application.Tests.Services;

public class AppointmentValidationServiceTests
{
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly AppointmentValidationService _service;

    public AppointmentValidationServiceTests()
    {
        _appointmentRepository = Substitute.For<IAppointmentRepository>();
        _service = new AppointmentValidationService(_appointmentRepository);
    }

    [Fact]
    public async Task GetAppointmentOrThrowAsync_WhenAppointmentExists_ReturnsAppointment()
    {
        // Arrange
        _dateTimeProvider.GetUtcNow().Returns(new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero));

        var appointmentDate = AppointmentDate.Create(new DateTime(2025, 1, 2, 9, 0, 0), _dateTimeProvider);
        var expectedAppointment = new Appointment(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), appointmentDate);
        _appointmentRepository.GetByIdAsync(expectedAppointment.Id).Returns(Task.FromResult(expectedAppointment));

        // Act
        var result = await _service.GetAppointmentOrThrowAsync(expectedAppointment.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedAppointment, result);
    }

    [Fact]
    public async Task GetAppointmentOrThrowAsync_WhenAppointmentNotExists_ThrowsAppointmentNotFoundException()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        _appointmentRepository.GetByIdAsync(appointmentId).Returns(Task.FromResult<Appointment>(null));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AppointmentNotFoundException>(() => _service.GetAppointmentOrThrowAsync(appointmentId));
        Assert.Equal($"Appointment with ID {appointmentId} not found.", exception.Message);
    }

    [Fact]
    public async Task EnsureDoctorHasNoConflictingAppointmentsAsync_WhenNoConflict_DoesNotThrow()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var appontmentDate = DateTimeOffset.Now;
        _appointmentRepository.HasConflictingAppointmentsForDoctorAsync(doctorId, appontmentDate).Returns(Task.FromResult(false));

        // Act & Assert
        await _service.EnsureDoctorHasNoConflictingAppointmentsAsync(doctorId, appontmentDate);
    }

    [Fact]
    public async Task EnsureDoctorHasNoConflictingAppointmentsAsync_WhenConflict_ThrowsConflict()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var date = DateTimeOffset.Now;
        _appointmentRepository.HasConflictingAppointmentsForDoctorAsync(doctorId, date)
                                    .Returns(Task.FromResult(true));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<AppointmentConflictException>(() => _service.EnsureDoctorHasNoConflictingAppointmentsAsync(doctorId, date));
        Assert.Equal(
            $"The doctor with ID '{doctorId}' already has an appointment scheduled at {date:yyyy-MM-dd HH:mm}.",
            ex.Message);
    }


    [Fact]
    public async Task EnsurePatientHasNoConflictingAppointmentsAsync_WhenNoConflict_DoesNotThrow()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var date = DateTimeOffset.Now;
        _appointmentRepository.HasConflictingAppointmentsForPatientAsync(patientId, date)
                              .Returns(Task.FromResult(false));

        // Act & Assert 
        await _service.EnsurePatientHasNoConflictingAppointmentsAsync(patientId, date);
    }

    [Fact]
    public async Task EnsurePatientHasNoConflictingAppointmentsAsync_WhenConflict_ThrowsConflict()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var date = DateTimeOffset.Now;
        _appointmentRepository.HasConflictingAppointmentsForPatientAsync(patientId, date)
                                    .Returns(Task.FromResult(true));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<AppointmentConflictException>(() => _service.EnsurePatientHasNoConflictingAppointmentsAsync(patientId, date));
        Assert.Equal(
            $"The patient with ID '{patientId}' already has an appointment scheduled at {date:yyyy-MM-dd HH:mm}.",
            ex.Message);
    }
}
