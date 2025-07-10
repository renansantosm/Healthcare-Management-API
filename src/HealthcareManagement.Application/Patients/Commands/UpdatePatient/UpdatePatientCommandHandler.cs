using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Domain.ValueObjects;
using MediatR;

namespace HealthcareManagement.Application.Patients.Commands.UpdatePatient;

public class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand, Unit>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IPatientValidationService _patientValidationService;

    public UpdatePatientCommandHandler(IPatientRepository patientRepository, IPatientValidationService patientValidationService)
    {
        _patientRepository = patientRepository;
        _patientValidationService = patientValidationService;
    }

    public async Task <Unit> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await _patientValidationService.GetPatientOrThrowAsync(request.Id);

        await _patientValidationService.CheckCpfUniqueAsync(request.Cpf);
        await _patientValidationService.CheckEmailUniqueAsync(request.Email);

        patient.Update(
            PersonInfo.Create(
                FullName.Create(request.FirstName, request.LastName),
                BirthDate.Create(request.BirthDate),
                Cpf.Create(request.Cpf),
                Email.Create(request.Email),
                MobilePhoneNumber.Create(request.PhoneNumber)
            )
        );

        await _patientRepository.UpdateAsync(patient);

        return Unit.Value;
    }
}
