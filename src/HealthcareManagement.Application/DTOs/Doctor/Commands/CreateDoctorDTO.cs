namespace HealthcareManagement.Application.DTOs.Doctor.Commands;

public record CreateDoctorDTO(
    string FirstName,
    string LastName,
    DateOnly BirthDate,
    string Cpf,
    string Email,
    string PhoneNumber);

