using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Exceptions.Shared;
using HealthcareManagement.Domain.Interfaces;

namespace HealthcareManagement.Application.Services.Validation;

public class PatientValidationService : IPatientValidationService
{
    private readonly IPatientRepository _patientRepository;

    public PatientValidationService(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }
    public async Task<Patient> GetPatientOrThrowAsync(Guid id)
    {
        var patient = await _patientRepository.GetByIdAsync(id);

        if (patient is null)
        {
            throw new PatientNotFoundException($"Patient with ID {id} not found.");
        }

        return patient;
    }
    public async Task CheckCpfUniqueAsync(string cpf)
    {
        var exists = await _patientRepository.CpfExistsAsync(cpf);

        if (exists)
        {
            throw new CpfNotUniqueException("CPF must be unique");
        }
    }

    public async Task CheckEmailUniqueAsync(string email)
    {
        var exists = await _patientRepository.EmailExistsAsync(email);

        if (exists)
        {
            throw new EmailNotUniqueException("Email must be unique");
        }
    }
}
