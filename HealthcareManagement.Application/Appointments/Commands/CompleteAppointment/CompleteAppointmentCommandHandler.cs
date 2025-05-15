using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Domain.Interfaces;
using MediatR;

namespace HealthcareManagement.Application.Appointments.Commands.Complete;

public class CompleteAppointmentCommandHandler : IRequestHandler<CompleteAppointmentCommand, Unit>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IAppointmentValidationService _appointmentValidationService;

    public CompleteAppointmentCommandHandler(IAppointmentRepository appointmentRepository, IAppointmentValidationService appointmentValidationService, IDateTimeProvider dateTimeProvider)
    {
        _appointmentRepository = appointmentRepository;
        _appointmentValidationService = appointmentValidationService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Unit> Handle(CompleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentValidationService.GetAppointmentOrThrowAsync(request.Id);

        appointment.Complete(_dateTimeProvider);

        await _appointmentRepository.UpdateAsync(appointment);

        return Unit.Value;
    }
}

