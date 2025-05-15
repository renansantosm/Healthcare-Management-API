using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Exceptions.Doctor;
using HealthcareManagement.Domain.Exceptions.Shared;
using HealthcareManagement.Domain.Interfaces;

namespace HealthcareManagement.Application.Services.Validation;

public class DoctorValidationService : IDoctorValidationService
{
    private readonly IDoctorRepository _doctorRepository;

    public DoctorValidationService(IDoctorRepository doctorRepository)
    {
        _doctorRepository = doctorRepository;
    }

    public async Task<Doctor> GetDoctorOrThrowAsync(Guid id)
    {
        var doctor = await _doctorRepository.GetByIdAsync(id);

        if (doctor is null)
        {
            throw new DoctorNotFoundException($"Doctor with ID {id} not found.");
        }

        return doctor;
    }

    public async Task CheckCpfUniqueAsync(string cpf)
    {
        var exists = await _doctorRepository.CpfExistsAsync(cpf);

        if (exists)
        {
            throw new CpfNotUniqueException("CPF must be unique");
        }
    }

    public async Task CheckEmailUniqueAsync(string email)
    {
        var exists = await _doctorRepository.EmailExistsAsync(email);

        if (exists)
        {
            throw new EmailNotUniqueException("Email must be unique");
        }
    }
}
