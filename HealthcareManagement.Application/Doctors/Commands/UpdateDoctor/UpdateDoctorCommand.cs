using MediatR;

namespace HealthcareManagement.Application.Doctors.Commands.UpdateDoctor;

public record UpdateDoctorCommand(
    Guid Id, 
    string FirstName, 
    string LastName,
    DateOnly Birthdate,
    string Cpf,
    string PhoneNumber, 
    string Email) : IRequest<Unit>;
