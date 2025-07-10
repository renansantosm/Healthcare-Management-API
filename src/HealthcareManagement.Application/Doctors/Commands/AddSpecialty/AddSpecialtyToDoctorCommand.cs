using MediatR;

namespace HealthcareManagement.Application.Doctors.Commands.AddSpecialty;

public record AddSpecialtyToDoctorCommand(
    Guid DoctorId,
    string Specialty) : IRequest<Unit>;
