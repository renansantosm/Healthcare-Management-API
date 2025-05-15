using HealthcareManagement.Domain.Entities;

namespace HealthcareManagement.Application.Services.Interfaces;

public interface IPatientValidationService
{
    Task<Patient> GetPatientOrThrowAsync(Guid id);
    Task CheckEmailUniqueAsync(string email);
    Task CheckCpfUniqueAsync(string cpf);
}
