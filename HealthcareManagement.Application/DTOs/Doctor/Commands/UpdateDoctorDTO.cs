namespace HealthcareManagement.Application.DTOs.Doctor.Commands;

public record UpdateDoctorDTO(
    Guid Id,
    string FirstName,
    string LastName,
    DateOnly BirthDate,
    string Cpf,
    string Email,
    string PhoneNumber);
