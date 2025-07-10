using HealthcareManagement.Application.DTOs.Appointment.Queries;
using MediatR;

namespace HealthcareManagement.Application.Appointments.Queries.GetAppointmentWithPrescription;

public record GetAppointmentWithPrescriptionQuery(Guid Id) : IRequest<AppointmentWithPrescriptionDTO?>;
