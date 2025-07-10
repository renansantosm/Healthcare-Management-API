using HealthcareManagement.Domain.Entities;

namespace HealthcareManagement.Domain.Interfaces;

public interface IPrescriptionRepository
{
    Task<IEnumerable<Prescription>> GetPrescriptionsAsync(int pageNumber, int pageSize);
    Task<Prescription?> GetByIdAsync(Guid id);
}
