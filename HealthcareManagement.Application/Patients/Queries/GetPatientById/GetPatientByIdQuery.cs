using HealthcareManagement.Application.DTOs.Patient.Queries;
using MediatR;

namespace HealthcareManagement.Application.Patients.Queries.GetPatientById;

public record GetPatientByIdQuery(Guid Id) : IRequest<PatientDTO?>;
