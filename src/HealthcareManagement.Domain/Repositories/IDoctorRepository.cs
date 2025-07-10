using HealthcareManagement.Domain.Entities;

namespace HealthcareManagement.Domain.Interfaces;

public interface IDoctorRepository
{
    Task<IEnumerable<Doctor>> GetDoctorsAsync(int pageNumber, int pageSize);
    Task<Doctor?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(Doctor doctor);
    Task<bool> CpfExistsAsync(string cpf);
    Task<bool> EmailExistsAsync(string email);
    Task UpdateAsync(Doctor doctor);
    Task DeleteAsync(Doctor doctor);
}
