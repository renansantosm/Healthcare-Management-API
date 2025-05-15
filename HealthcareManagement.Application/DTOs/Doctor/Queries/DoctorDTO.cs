namespace HealthcareManagement.Application.DTOs.Doctor.Queries;

public record DoctorDTO(
    Guid Id, 
    string FirstName, 
    string LastName,
    DateOnly BirthDate,
    string Cpf,
    string Email,
    string PhoneNumber,
    List<string> Specialties);
