using MediatR;

namespace HealthcareManagement.Application.Patients.Commands.RemovePatient;

public record RemovePatientCommand(Guid Id) : IRequest<Unit>;
