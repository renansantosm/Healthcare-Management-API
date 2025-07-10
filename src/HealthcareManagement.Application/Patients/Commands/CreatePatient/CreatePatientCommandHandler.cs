using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Domain.ValueObjects;
using MediatR;

namespace HealthcareManagement.Application.Patients.Commands.CreatePatient;

public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, Guid>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IPatientValidationService _patientValidationService;

    public CreatePatientCommandHandler(IPatientRepository patientRepository, IPatientValidationService patientValidationService)
    {
        _patientRepository = patientRepository;
        _patientValidationService = patientValidationService;
    }

    public async Task<Guid> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        await _patientValidationService.CheckCpfUniqueAsync(request.Cpf);
        await _patientValidationService.CheckEmailUniqueAsync(request.Email);

        var patient = new Patient(
            Guid.NewGuid(),
            PersonInfo.Create(
                FullName.Create(request.FirstName, request.LastName),
                BirthDate.Create(request.Birthdate),
                Cpf.Create(request.Cpf),
                Email.Create(request.Email),
                MobilePhoneNumber.Create(request.PhoneNumber)
            )
        );

        var id = await _patientRepository.CreateAsync(patient);

        return id;
    }
}
