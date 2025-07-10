using MediatR;

namespace HealthcareManagement.Application.Patients.Commands.UpdatePatient;

public record UpdatePatientCommand(
    Guid Id, 
    string FirstName, 
    string LastName, 
    DateOnly BirthDate, 
    string Cpf, 
    string PhoneNumber, 
    string Email) : IRequest<Unit>;