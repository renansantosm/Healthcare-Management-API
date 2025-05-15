using MediatR;

namespace HealthcareManagement.Application.Appointments.Commands.RescheduleAppointment;

public record RescheduleAppointmentCommand(
    Guid Id, 
    DateTimeOffset NewAppointmentDate) : IRequest<Unit>;
