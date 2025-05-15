using HealthcareManagement.Application.DTOs.Appointment.Queries;
using MediatR;

namespace HealthcareManagement.Application.Appointments.Queries.GetAppointments;

public record GetAppointmentsQuery(
    int PageNumber, 
    int PageSize) : IRequest<IEnumerable<AppointmentDTO>>;
