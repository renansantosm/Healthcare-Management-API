using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Domain.ValueObjects;
using MediatR;

namespace HealthcareManagement.Application.Appointments.Commands.Create;

public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, Guid>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IDoctorValidationService _doctorValidationService;
    private readonly IPatientValidationService _patientValidationService;
    private readonly IAppointmentValidationService _appointmentValidationService;

    public CreateAppointmentCommandHandler(IAppointmentRepository appointmentRepository,
        IDateTimeProvider dateTimeProvider,
        IDoctorValidationService doctorValidationService,
        IPatientValidationService patientValidationService,
        IAppointmentValidationService appointmentValidationService)
    {
        _appointmentRepository = appointmentRepository;
        _dateTimeProvider = dateTimeProvider;
        _doctorValidationService = doctorValidationService;
        _patientValidationService = patientValidationService;
        _appointmentValidationService = appointmentValidationService;
    }

    public async Task<Guid> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        _ = await _doctorValidationService.GetDoctorOrThrowAsync(request.DoctorId);
        await _appointmentValidationService.EnsureDoctorHasNoConflictingAppointmentsAsync(request.DoctorId, request.AppointmentDate);

        _ = await _patientValidationService.GetPatientOrThrowAsync(request.PatientId);
        await _appointmentValidationService.EnsurePatientHasNoConflictingAppointmentsAsync(request.PatientId, request.AppointmentDate);

        var appointment = new Appointment(
            Guid.NewGuid(),
            request.DoctorId,
            request.PatientId,
            AppointmentDate.Create(request.AppointmentDate.ToUniversalTime(), _dateTimeProvider)
        );

        var id = await _appointmentRepository.CreateAsync(appointment);

        return id;
    }
}
