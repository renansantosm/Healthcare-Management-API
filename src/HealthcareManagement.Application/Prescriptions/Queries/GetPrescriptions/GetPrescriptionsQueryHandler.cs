using HealthcareManagement.Application.DTOs.Prescription.Queries;
using HealthcareManagement.Domain.Interfaces;
using MediatR;

namespace HealthcareManagement.Application.Prescriptions.Queries.GetPrescriptions;

public class GetPrescriptionsQueryHandler : IRequestHandler<GetPrescriptionsQuery, IEnumerable<PrescriptionDTO>>
{
    private readonly IPrescriptionRepository _prescriptionRepository;

    public GetPrescriptionsQueryHandler(IPrescriptionRepository prescriptionRepository)
    {
        _prescriptionRepository = prescriptionRepository;
    }

    public async Task<IEnumerable<PrescriptionDTO>> Handle(GetPrescriptionsQuery request, CancellationToken cancellationToken)
    {
        var prescriptions = await _prescriptionRepository.GetPrescriptionsAsync(request.PageNumber, request.PageSize);

        return prescriptions.Select(prescription => new PrescriptionDTO
        (
            prescription.Id,
            prescription.AppointmentId,
            prescription.Medication,
            prescription.Dosage,
            prescription.Duration,
            prescription.Instructions
        )).ToList();
    }
}
