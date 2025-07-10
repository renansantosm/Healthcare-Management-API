using HealthcareManagement.Application.DTOs.Patient.Queries;
using HealthcareManagement.Application.Services.Interfaces;
using MediatR;

namespace HealthcareManagement.Application.Patients.Queries.GetPatientById;

public class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, PatientDTO?>
{
    private readonly IPatientValidationService _patientValidationService;

    public GetPatientByIdQueryHandler(IPatientValidationService patientValidationService)
    {
        _patientValidationService = patientValidationService;
    }

    public async Task<PatientDTO?> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        var patient = await _patientValidationService.GetPatientOrThrowAsync(request.Id);

        return new PatientDTO
        (
            patient.Id,
            patient.PersonInfo.FullName.FirstName,
            patient.PersonInfo.FullName.LastName,
            patient.PersonInfo.BirthDate.Date,
            patient.PersonInfo.Cpf.Number,
            patient.PersonInfo.Email.Adress,
            patient.PersonInfo.MobilePhoneNumber.Number      
        );
    }
}
