namespace HealthcareManagement.Application.DTOs.Doctor.Commands;

public record RemoveSpecialtyFromDoctorDTO(
    Guid DoctorId,
    string Specialty);
