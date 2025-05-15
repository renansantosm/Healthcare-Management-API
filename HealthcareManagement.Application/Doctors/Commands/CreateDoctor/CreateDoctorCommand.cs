using MediatR;

namespace HealthcareManagement.Application.Doctors.Commands.CreateDoctor;

public record CreateDoctorCommand(
    string FirstName, 
    string LastName, 
    DateOnly Birthdate,
    string Cpf,
    string PhoneNumber, 
    string Email) : IRequest<Guid>;
