using HealthcareManagement.Application.DTOs.Patient.Queries;
using MediatR;

namespace HealthcareManagement.Application.Patients.Queries.GetPatients;

public record GetPatientsQuery(
    int PageNumber, 
    int PageSize) : IRequest<IEnumerable<PatientDTO>>;

