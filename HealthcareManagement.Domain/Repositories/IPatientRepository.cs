using HealthcareManagement.Domain.Entities;

namespace HealthcareManagement.Domain.Interfaces;

public interface IPatientRepository
{
    Task<IEnumerable<Patient>> GetPatientsAsync(int pageNumber, int pageSize);
    Task<Patient?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(Patient patient);
    Task<bool> CpfExistsAsync(string cpf);
    Task<bool> EmailExistsAsync(string email);
    Task UpdateAsync(Patient patient);
    Task DeleteAsync(Patient patient);
}
