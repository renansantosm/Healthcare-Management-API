using HealthcareManagement.Application.DTOs.Prescription.Queries;
using MediatR;

namespace HealthcareManagement.Application.Prescriptions.Queries.GetPrescriptions;

public record GetPrescriptionsQuery(
    int PageNumber, 
    int PageSize) : IRequest<IEnumerable<PrescriptionDTO>>;