using MediatR;

namespace HealthcareManagement.Application.Doctors.Commands.RemoveDoctor;

public record RemoveDoctorCommand(Guid Id) : IRequest<Unit>;