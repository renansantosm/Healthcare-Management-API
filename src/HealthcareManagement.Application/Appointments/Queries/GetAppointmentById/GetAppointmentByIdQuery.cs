using HealthcareManagement.Application.DTOs.Appointment.Queries;
using MediatR;

namespace HealthcareManagement.Application.Appointments.Queries.GetAppointmentById;

public record GetAppointmentByIdQuery(Guid Id) : IRequest<AppointmentDTO?>;
