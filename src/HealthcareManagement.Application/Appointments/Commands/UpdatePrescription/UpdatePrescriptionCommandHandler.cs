using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Domain.Interfaces;
using MediatR;

namespace HealthcareManagement.Application.Appointments.Commands.UpdatePrescription;

public class UpdatePrescriptionCommandHandler : IRequestHandler<UpdatePrescriptionCommand, Unit>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentValidationService _appointmentValidationService;

    public UpdatePrescriptionCommandHandler(IAppointmentRepository appointmentRepository, IAppointmentValidationService appointmentValidationService)
    {
        _appointmentRepository = appointmentRepository;
        _appointmentValidationService = appointmentValidationService;
    }

    public async Task<Unit> Handle(UpdatePrescriptionCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentValidationService.GetAppointmentWithPrescriptionsOrThrowAsync(request.AppointmentId);

        appointment.UpdatePrescription(
            request.PrescriptionId,
            request.Medication,
            request.Dosage,
            request.Duration,
            request.Instructions
        );

        await _appointmentRepository.UpdateAsync(appointment);

        return Unit.Value;
    }
}

