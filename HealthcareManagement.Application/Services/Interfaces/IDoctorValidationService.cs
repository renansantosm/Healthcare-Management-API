using HealthcareManagement.Domain.Entities;

namespace HealthcareManagement.Application.Services.Interfaces;

public interface IDoctorValidationService
{
    Task<Doctor> GetDoctorOrThrowAsync(Guid id);
    Task CheckEmailUniqueAsync(string email);
    Task CheckCpfUniqueAsync(string cpf);
}