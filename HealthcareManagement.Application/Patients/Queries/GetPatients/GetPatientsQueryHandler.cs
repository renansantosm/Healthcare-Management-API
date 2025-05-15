using HealthcareManagement.Application.DTOs.Patient.Queries;
using HealthcareManagement.Domain.Interfaces;
using MediatR;

namespace HealthcareManagement.Application.Patients.Queries.GetPatients;

public class GetPatientsQueryHandler : IRequestHandler<GetPatientsQuery, IEnumerable<PatientDTO>>
{
    private readonly IPatientRepository _patientRepository;
    public GetPatientsQueryHandler(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<IEnumerable<PatientDTO>> Handle(GetPatientsQuery request, CancellationToken cancellationToken)
    {
      var patients = await _patientRepository.GetPatientsAsync(request.PageNumber, request.PageSize);

        return patients.Select(patient => new PatientDTO
        (
            patient.Id,
            patient.PersonInfo.FullName.FirstName,
            patient.PersonInfo.FullName.LastName,
            patient.PersonInfo.BirthDate.Date,
            patient.PersonInfo.Cpf.Number,
            patient.PersonInfo.Email.Adress,
            patient.PersonInfo.MobilePhoneNumber.Number
        )).ToList();
    }
}

