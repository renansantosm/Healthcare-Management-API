namespace HealthcareManagement.Application.DTOs.Prescription.Queries;

public record PrescriptionDTO(
    Guid Id,
    Guid AppointmentId, 
    string Medication, 
    string Dosage, 
    string Duration, 
    string Instructions);

