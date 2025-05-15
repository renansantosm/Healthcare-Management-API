using MediatR;

namespace HealthcareManagement.Application.Appointments.Commands.Complete;

public record CompleteAppointmentCommand(Guid Id) : IRequest<Unit>;

