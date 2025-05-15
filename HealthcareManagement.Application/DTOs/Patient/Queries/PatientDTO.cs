namespace HealthcareManagement.Application.DTOs.Patient.Queries;

public record PatientDTO(
    Guid Id,
    string FirstName,
    string LastName,
    DateOnly BirthDate,
    string Cpf,
    string Email,
    string PhoneNumber
    );
