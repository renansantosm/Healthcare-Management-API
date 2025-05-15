namespace HealthcareManagement.Application.DTOs.Patient.Commands;

public record UpdatePatientDTO(
    Guid Id,
    string FirstName,
    string LastName,
    DateOnly BirthDate,
    string Cpf,
    string Email,
    string PhoneNumber);
