namespace HealthcareManagement.Application.DTOs.Patient.Commands;

public record CreatePatientDTO(
    string FirstName,
    string LastName,
    DateOnly BirthDate,
    string Cpf,
    string Email,
    string PhoneNumber);
