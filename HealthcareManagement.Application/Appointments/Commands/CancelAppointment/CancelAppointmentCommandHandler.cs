using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Domain.Interfaces;
using MediatR;

namespace HealthcareManagement.Application.Appointments.Commands.CancelAppointment;

public class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand, Unit>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentValidationService _appointmentValidationService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CancelAppointmentCommandHandler(IAppointmentRepository appointmentRepository, IAppointmentValidationService appointmentValidationService, IDateTimeProvider dateTimeProvider)
    {
        _appointmentRepository = appointmentRepository;
        _appointmentValidationService = appointmentValidationService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Unit> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentValidationService.GetAppointmentOrThrowAsync(request.Id);

        appointment.Cancel(_dateTimeProvider);

        await _appointmentRepository.UpdateAsync(appointment);

        return Unit.Value;
    }
}
