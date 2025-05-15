using HealthcareManagement.Domain.Entities;

namespace HealthcareManagement.Application.Services.Interfaces;

public interface IPrescriptionValidationService
{
    Task<Prescription> GetPrescriptionOrThrowAsync(Guid id);
}
