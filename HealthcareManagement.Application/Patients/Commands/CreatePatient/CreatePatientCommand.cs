using MediatR;

namespace HealthcareManagement.Application.Patients.Commands.CreatePatient;

public record CreatePatientCommand(
    string FirstName, 
    string LastName,
    DateOnly Birthdate,
    string Cpf,
    string PhoneNumber, 
    string Email) : IRequest<Guid>;
