using HealthcareManagement.Application.Appointments.Commands.Create;
using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Exceptions.Appointment;
using HealthcareManagement.Domain.Exceptions.Doctor;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Domain.ValueObjects;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace HealthcareManagement.Application.Tests.Appointments.Commands.Create;

public class CreateAppointmentCommandHandlerTests
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IDoctorValidationService _doctorValidationService;
    private readonly IPatientValidationService _patientValidationService;
    private readonly IAppointmentValidationService _appointmentValidationService;
    private readonly CreateAppointmentCommandHandler _handler;
    private readonly PersonInfo _personInfo = PersonInfo.Create(
        FullName.Create("John", "Doe"),
        BirthDate.Create(new DateOnly(1990, 03, 01)),
        Cpf.Create("12345678909"),
        Email.Create("john_doe@email.com"),
        MobilePhoneNumber.Create("11987654321"));

    public CreateAppointmentCommandHandlerTests()
    {
        _appointmentRepository = Substitute.For<IAppointmentRepository>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        _doctorValidationService = Substitute.For<IDoctorValidationService>();
        _patientValidationService = Substitute.For<IPatientValidationService>();
        _appointmentValidationService = Substitute.For<IAppointmentValidationService>();

        _handler = new CreateAppointmentCommandHandler(
            _appointmentRepository,
            _dateTimeProvider,
            _doctorValidationService,
            _patientValidationService,
            _appointmentValidationService
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesAppointmentAndReturnsId()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var patientId = Guid.NewGuid();

        _dateTimeProvider.GetUtcNow().Returns(new DateTimeOffset(2025, 5, 1, 8, 0, 0, TimeSpan.Zero));

        var appointmentDate = new DateTimeOffset(2025, 05, 01, 9, 0, 0, TimeSpan.Zero);

        _doctorValidationService.GetDoctorOrThrowAsync(doctorId)
            .Returns(Task.FromResult(new Doctor(doctorId, _personInfo)));

        _patientValidationService.GetPatientOrThrowAsync(patientId)
            .Returns(Task.FromResult(new Patient(patientId, _personInfo)));

        _appointmentValidationService.EnsureDoctorHasNoConflictingAppointmentsAsync(doctorId, appointmentDate).Returns(Task.CompletedTask);
        _appointmentValidationService.EnsurePatientHasNoConflictingAppointmentsAsync(patientId, appointmentDate).Returns(Task.CompletedTask);

        Appointment capturedAppointment = null;
        var expectedId = Guid.NewGuid();
        _appointmentRepository.CreateAsync(Arg.Do<Appointment>(a => capturedAppointment = a))
            .Returns(Task.FromResult(expectedId));

        var command = new CreateAppointmentCommand(doctorId, patientId, appointmentDate);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedId, result);
        await _doctorValidationService.Received(1).GetDoctorOrThrowAsync(doctorId);
        await _appointmentValidationService.Received(1)
            .EnsureDoctorHasNoConflictingAppointmentsAsync(doctorId, appointmentDate);
        await _patientValidationService.Received(1).GetPatientOrThrowAsync(patientId);
        await _appointmentValidationService.Received(1)
            .EnsurePatientHasNoConflictingAppointmentsAsync(patientId, appointmentDate);
        await _appointmentRepository.Received(1).CreateAsync(Arg.Any<Appointment>());

        Assert.NotNull(capturedAppointment);
        Assert.Equal(doctorId, capturedAppointment.DoctorId);
        Assert.Equal(patientId, capturedAppointment.PatientId);
        Assert.Equal(appointmentDate.ToUniversalTime(), capturedAppointment.AppointmentDate.Date);
    }

    [Fact]
    public async Task Handle_DoctorNotFound_ThrowsExceptionAndDoesNotCallRepository() 
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var appointmentDate = DateTimeOffset.UtcNow.AddDays(1);

        _doctorValidationService.GetDoctorOrThrowAsync(doctorId)
            .ThrowsAsync(new DoctorNotFoundException($"Doctor with ID {doctorId} not found."));

        var command = new CreateAppointmentCommand(doctorId, patientId, appointmentDate);

        // Act & Assert
        await Assert.ThrowsAsync<DoctorNotFoundException>(() => _handler.Handle(command, CancellationToken.None));

        await _doctorValidationService.Received(1).GetDoctorOrThrowAsync(doctorId);
        await _appointmentValidationService.DidNotReceive()
            .EnsureDoctorHasNoConflictingAppointmentsAsync(doctorId, appointmentDate);
        await _patientValidationService.DidNotReceive().GetPatientOrThrowAsync(patientId);
        await _appointmentValidationService.DidNotReceive()
            .EnsurePatientHasNoConflictingAppointmentsAsync(patientId, appointmentDate);
        await _appointmentRepository.DidNotReceive().CreateAsync(Arg.Any<Appointment>());
    }

    [Fact]
    public async Task Handle_DoctorConflict_ThrowsExceptionAndDoesNotCallRepository() 
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var patientId = Guid.NewGuid();

        _dateTimeProvider.GetUtcNow().Returns(new DateTimeOffset(2025, 5, 1, 8, 0, 0, TimeSpan.Zero));

        var appointmentDate = new DateTimeOffset(2025, 05, 01, 9, 0, 0, TimeSpan.Zero);

        _doctorValidationService.GetDoctorOrThrowAsync(doctorId)
            .Returns(Task.FromResult(new Doctor(doctorId, _personInfo)));
        _appointmentValidationService.EnsureDoctorHasNoConflictingAppointmentsAsync(doctorId, appointmentDate)
            .ThrowsAsync(new AppointmentConflictException($"The doctor with ID '{doctorId}' already has an appointment scheduled at {appointmentDate:yyyy-MM-dd HH:mm}."));

        var command = new CreateAppointmentCommand(doctorId, patientId, appointmentDate);

        // Act & Assert
        await Assert.ThrowsAsync<AppointmentConflictException>(() => _handler.Handle(command, CancellationToken.None));

        await _doctorValidationService.Received(1).GetDoctorOrThrowAsync(doctorId);
        await _appointmentValidationService.Received(1)
            .EnsureDoctorHasNoConflictingAppointmentsAsync(doctorId, appointmentDate);
        await _patientValidationService.DidNotReceive().GetPatientOrThrowAsync(patientId);
        await _appointmentValidationService.DidNotReceive()
            .EnsurePatientHasNoConflictingAppointmentsAsync(patientId, appointmentDate);
        await _appointmentRepository.DidNotReceive().CreateAsync(Arg.Any<Appointment>());
    }

    [Fact]
    public async Task Handle_PatientNotFound_ThrowsExceptionAndDoesNotCallRepository() 
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var patientId = Guid.NewGuid();

        _dateTimeProvider.GetUtcNow().Returns(new DateTimeOffset(2025, 5, 1, 8, 0, 0, TimeSpan.Zero));

        var appointmentDate = new DateTimeOffset(2025, 05, 01, 9, 0, 0, TimeSpan.Zero);

        _doctorValidationService.GetDoctorOrThrowAsync(doctorId)
            .Returns(Task.FromResult(new Doctor(doctorId, _personInfo)));

        _appointmentValidationService.EnsureDoctorHasNoConflictingAppointmentsAsync(doctorId, appointmentDate)
            .Returns(Task.CompletedTask);

        _patientValidationService.GetPatientOrThrowAsync(patientId)
            .Throws(new AppointmentConflictException($"The patient with ID '{patientId}' already has an appointment scheduled at {appointmentDate:yyyy-MM-dd HH:mm}."));

        var command = new CreateAppointmentCommand(doctorId, patientId, appointmentDate);

        // Act & Assert
        await Assert.ThrowsAsync<AppointmentConflictException>(() => _handler.Handle(command, CancellationToken.None));

        await _doctorValidationService.Received(1).GetDoctorOrThrowAsync(doctorId);
        await _appointmentValidationService.Received(1)
            .EnsureDoctorHasNoConflictingAppointmentsAsync(doctorId, appointmentDate);
        await _patientValidationService.Received(1).GetPatientOrThrowAsync(patientId);
        await _appointmentValidationService.DidNotReceive()
            .EnsurePatientHasNoConflictingAppointmentsAsync(patientId, appointmentDate);
        await _appointmentRepository.DidNotReceive().CreateAsync(Arg.Any<Appointment>());
    }

    [Fact]
    public async Task Handle_PatientConflict_ThrowsExceptionAndDoesNotCallRepository()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var patientId = Guid.NewGuid();

        _dateTimeProvider.GetUtcNow().Returns(new DateTimeOffset(2025, 5, 1, 8, 0, 0, TimeSpan.Zero));

        var appointmentDate = new DateTimeOffset(2025, 05, 01, 9, 0, 0, TimeSpan.Zero);

        _doctorValidationService.GetDoctorOrThrowAsync(doctorId).Returns(Task.FromResult(new Doctor(doctorId, _personInfo)));
        _appointmentValidationService.EnsureDoctorHasNoConflictingAppointmentsAsync(doctorId, appointmentDate).Returns(Task.CompletedTask);
        _patientValidationService.GetPatientOrThrowAsync(patientId).Returns(Task.FromResult(new Patient(patientId, _personInfo)));
        _appointmentValidationService.EnsurePatientHasNoConflictingAppointmentsAsync(patientId, appointmentDate)
            .Throws(new AppointmentConflictException($"The patient with ID '{patientId}' already has an appointment scheduled at {appointmentDate:yyyy-MM-dd HH:mm}."));

        var command = new CreateAppointmentCommand(doctorId, patientId, appointmentDate);

        // Act & Assert
        await Assert.ThrowsAsync<AppointmentConflictException>(() => _handler.Handle(command, CancellationToken.None));
        await _doctorValidationService.Received(1).GetDoctorOrThrowAsync(doctorId);
        await _appointmentValidationService.Received(1)
            .EnsureDoctorHasNoConflictingAppointmentsAsync(doctorId, appointmentDate);
        await _patientValidationService.Received(1).GetPatientOrThrowAsync(patientId);
        await _appointmentValidationService.Received(1)
            .EnsurePatientHasNoConflictingAppointmentsAsync(patientId, appointmentDate);
        await _appointmentRepository.DidNotReceive().CreateAsync(Arg.Any<Appointment>());
    }
}
