using HealthcareManagement.Application.DTOs.Prescription.Queries;
using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Domain.Interfaces;
using MediatR;

namespace HealthcareManagement.Application.Prescriptions.Queries.GetPrescriptionById;

public class GetPrescriptionByIdQueryHandler : IRequestHandler<GetPrescriptionByIdQuery, PrescriptionDTO?>
{
    private readonly IPrescriptionValidationService _prescriptionValidationService;

    public GetPrescriptionByIdQueryHandler(IPrescriptionValidationService prescriptionValidationService)
    {
        _prescriptionValidationService = prescriptionValidationService;
    }

    public async Task<PrescriptionDTO?> Handle(GetPrescriptionByIdQuery request, CancellationToken cancellationToken)
    {
        var prescription = await _prescriptionValidationService.GetPrescriptionOrThrowAsync(request.Id);

        return new PrescriptionDTO
        (
            prescription.Id,
            prescription.AppointmentId,
            prescription.Medication,
            prescription.Dosage,
            prescription.Duration,
            prescription.Instructions
        );
    }
}
