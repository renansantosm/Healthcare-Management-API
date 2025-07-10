using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Domain.Interfaces;
using MediatR;

namespace HealthcareManagement.Application.Patients.Commands.RemovePatient;

public class RemovePatientCommandHandler : IRequestHandler<RemovePatientCommand, Unit>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IPatientValidationService _patientValidationService;

    public RemovePatientCommandHandler(IPatientRepository patientRepository, IPatientValidationService patientValidationService)
    {
        _patientRepository = patientRepository;
        _patientValidationService = patientValidationService;
    }

    public async Task<Unit> Handle(RemovePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await _patientValidationService.GetPatientOrThrowAsync(request.Id);

        await _patientRepository.DeleteAsync(patient);

        return Unit.Value;
    }
}
