using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Exceptions.Prescription;
using HealthcareManagement.Domain.Interfaces;

namespace HealthcareManagement.Application.Services.Validation;

public class PrescriptionValidationService : IPrescriptionValidationService
{
    private readonly IPrescriptionRepository _prescriptionRepository;

    public PrescriptionValidationService(IPrescriptionRepository prescriptionRepository)
    {
        _prescriptionRepository = prescriptionRepository;
    }

    public async Task<Prescription> GetPrescriptionOrThrowAsync(Guid id)
    {
        var prescription = await _prescriptionRepository.GetByIdAsync(id);

        if (prescription is null)
        {
            throw new PrescriptionNotFoundException($"Prescription with ID {id} not found.");
        }

        return prescription;
    }
}
