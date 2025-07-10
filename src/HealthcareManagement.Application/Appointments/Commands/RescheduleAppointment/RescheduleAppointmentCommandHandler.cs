using HealthcareManagement.Application.Appointments.Commands.RescheduleAppointment;
using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Domain.ValueObjects;
using MediatR;

namespace HealthcareManagement.Application.Appointments.Commands.Reschedule;

public class RescheduleAppointmentCommandHandler : IRequestHandler<RescheduleAppointmentCommand, Unit>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IAppointmentValidationService _appointmentValidationService;

    public RescheduleAppointmentCommandHandler(IAppointmentRepository appointmentRepository, IDateTimeProvider dateTimeProvider, IAppointmentValidationService appointmentValidationService)
    {
        _appointmentRepository = appointmentRepository;
        _dateTimeProvider = dateTimeProvider;
        _appointmentValidationService = appointmentValidationService;
    }

    public async Task<Unit> Handle(RescheduleAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentValidationService.GetAppointmentOrThrowAsync(request.Id);

        await _appointmentValidationService.EnsureDoctorHasNoConflictingAppointmentsAsync(appointment.DoctorId, request.NewAppointmentDate.ToUniversalTime());
        await _appointmentValidationService.EnsurePatientHasNoConflictingAppointmentsAsync(appointment.PatientId, request.NewAppointmentDate.ToUniversalTime());

        appointment.Reschedule(AppointmentDate.Create(request.NewAppointmentDate.ToUniversalTime(), _dateTimeProvider));

        await _appointmentRepository.UpdateAsync(appointment);

        return Unit.Value;
    }
}
