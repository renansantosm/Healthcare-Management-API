using HealthcareManagement.Application.DTOs.Prescription.Queries;
using MediatR;

namespace HealthcareManagement.Application.Prescriptions.Queries.GetPrescriptionById;

public record GetPrescriptionByIdQuery(Guid Id) : IRequest<PrescriptionDTO?>;
