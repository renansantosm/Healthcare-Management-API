using HealthcareManagement.Domain.Enums;
using MediatR;

namespace HealthcareManagement.Application.Appointments.Commands.Create;

public record CreateAppointmentCommand(
    Guid DoctorId,
    Guid PatientId, 
    DateTimeOffset AppointmentDate) : IRequest<Guid>;

