using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Enums;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Domain.ValueObjects;
using HealthcareManagement.Infra.Data.Context;
using HealthcareManagement.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace HealthcareManagement.Infra.Data.Tests.Repositories;

public class AppointmentRepositoryTests
{
    private readonly IAppointmentRepository _repository;
    private readonly AppDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly Guid _doctorId;
    private readonly Guid _patientId;

    public AppointmentRepositoryTests()
    {
        // Um banco em memória com nome único (GUID) garante isolamento entre testes
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _repository = new AppointmentRepository(_dbContext, _dateTimeProvider);

        _doctorId = Guid.NewGuid();
        _patientId = Guid.NewGuid();
    }

    private async Task SeedDatabaseAsync()
    {
        var doctor1 = new Doctor(
            _doctorId,
            PersonInfo.Create(FullName.Create("John", "Doe"),
                BirthDate.Create(new DateOnly(1990, 10, 21)),
                Cpf.Create("12345678901"),
                Email.Create("john_doe@email.com"),
                MobilePhoneNumber.Create("11987654321")));

        var patient1 = new Patient(
            _patientId,
            PersonInfo.Create(FullName.Create("Fulano", "Da Silva"),
                BirthDate.Create(new DateOnly(2000, 11, 21)),
                Cpf.Create("12345678903"),
                Email.Create("fulano_doe@email.com"),
                MobilePhoneNumber.Create("11987651235")));


        _dbContext.Doctors.Add(doctor1);
        _dbContext.Patients.Add(patient1);

        await _dbContext.SaveChangesAsync();

        _dateTimeProvider.GetUtcNow().Returns(new DateTimeOffset(2025, 1, 1, 8, 0, 0, TimeSpan.Zero));

        var appointment = new Appointment(
            Guid.NewGuid(),
            doctor1.Id,
            patient1.Id,
            AppointmentDate.Create(new DateTime(2025, 1, 2, 9, 0, 0), _dateTimeProvider));

        _dbContext.Appointments.Add(appointment);
        await _dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task HasConflictingAppointmentsForDoctorAsync_WithConflict_ShouldReturnTrue()
    {
        // Arrange
        await SeedDatabaseAsync();

        var conflictingDateTime = new DateTime(2025, 1, 2, 9, 15, 0);
        var appointmentDate = AppointmentDate.Create(conflictingDateTime, _dateTimeProvider);

        // Act
        var hasConflict = await _repository.HasConflictingAppointmentsForDoctorAsync(_doctorId, appointmentDate.Date);

        // Assert
        Assert.True(hasConflict);
    }

    [Fact]
    public async Task HasConflictingAppointmentsForDoctorAsync_WithoutConflict_ShouldReturnFalse()
    {
        // Arrange
        await SeedDatabaseAsync();

        var conflictingDateTime = new DateTime(2025, 1, 2, 9, 31, 0);
        var appointmentDate = AppointmentDate.Create(conflictingDateTime, _dateTimeProvider);

        // Act
        var hasConflict = await _repository.HasConflictingAppointmentsForDoctorAsync(_doctorId, appointmentDate.Date);

        // Assert
        Assert.False(hasConflict);
    }

    [Fact]
    public async Task HasConflictingAppointmentsForPatientAsync_WithConflict_ShouldReturnTrue()
    {
        // Arrange
        await SeedDatabaseAsync();

        var conflictingDateTime = new DateTime(2025, 1, 2, 9, 15, 0);
        var appointmentDate = AppointmentDate.Create(conflictingDateTime, _dateTimeProvider);

        // Act
        var hasConflict = await _repository.HasConflictingAppointmentsForPatientAsync(_patientId, appointmentDate.Date);

        // Assert
        Assert.True(hasConflict);
    }

    [Fact]
    public async Task HasConflictingAppointmentsForPatientAsync_WithoutConflict_ShouldReturnFalse()
    {
        // Arrange
        await SeedDatabaseAsync();

        var conflictingDateTime = new DateTime(2025, 1, 2, 10, 30, 0);
        var appointmentDate = AppointmentDate.Create(conflictingDateTime, _dateTimeProvider);

        // Act
        var hasConflict = await _repository.HasConflictingAppointmentsForPatientAsync(_patientId, appointmentDate.Date);

        // Assert
        Assert.False(hasConflict);
    }

    [Fact]
    public async Task CanCancelAppointment_WithValidConditions_ShouldReturnTrue()
    {
        // Arrange
        await SeedDatabaseAsync();

        var appointment = await _dbContext.Appointments.FirstAsync();

        // Act
        var canCancel = await _repository.CanCancelAppointment(appointment.Id); // Cancelando a consulta

        // Assert
        Assert.True(canCancel);
    }

    [Fact]
    public async Task CanCancelAppointment_WithInvalidConditions_ShouldReturnFalse()
    {
        // Arrange
        await SeedDatabaseAsync();

        var soonAppointment = new Appointment(
                Guid.NewGuid(),
                _doctorId,
                _patientId,
                AppointmentDate.Create(new DateTime(2025, 1, 1, 12, 0, 0), _dateTimeProvider) 
            );

        _dbContext.Appointments.Add(soonAppointment);

        await _dbContext.SaveChangesAsync();

        // Act
        var canCancel = await _repository.CanCancelAppointment(soonAppointment.Id); // O horario "atual" foi definido no metodo SeedDatabaseAsync - 2025-01-01 08:00:00

        // Assert
        Assert.False(canCancel); 
    }
}
