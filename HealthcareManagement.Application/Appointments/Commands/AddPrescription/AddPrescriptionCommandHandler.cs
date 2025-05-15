using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Interfaces;
using MediatR;

namespace HealthcareManagement.Application.Appointments.Commands.AddPrescription;

public class AddPrescriptionCommandHandler : IRequestHandler<AddPrescriptionCommand, Guid>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentValidationService _appointmentValidationService;

    public AddPrescriptionCommandHandler(IAppointmentRepository appointmentRepository, IAppointmentValidationService appointmentValidationService)
    {
        _appointmentRepository = appointmentRepository;
        _appointmentValidationService = appointmentValidationService;
    }

    public async Task<Guid> Handle(AddPrescriptionCommand request, CancellationToken cancellationToken)
    {
        await _appointmentValidationService.ValidateAppointmentExistsOrThrowAsync(request.AppointmentId);

        var prescription = new Prescription(
            Guid.NewGuid(),
            request.AppointmentId,
            request.Medication,
            request.Dosage,
            request.Duration,
            request.Instructions);

        await _appointmentRepository.AddPrescriptionAsync(request.AppointmentId, prescription);

        return prescription.Id;
    }
}
