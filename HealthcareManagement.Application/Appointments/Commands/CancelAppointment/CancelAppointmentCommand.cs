using MediatR;

namespace HealthcareManagement.Application.Appointments.Commands.CancelAppointment;

public record CancelAppointmentCommand(Guid Id) : IRequest<Unit>;
