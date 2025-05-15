using MediatR;

namespace HealthcareManagement.Application.Doctors.Commands.RemoveSpecialty;

public record RemoveSpecialtyFromDoctorCommand(
    Guid DoctorId,
    string Specialty) : IRequest<Unit>;