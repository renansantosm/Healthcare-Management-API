namespace HealthcareManagement.Application.DTOs.Doctor.Commands;

public record AddSpecialtyToDoctorDTO(
    Guid DoctorId,
    string Specialty);

