namespace HealthcareManagement.Application.DTOs.Prescription.Queries;

public record PrescriptionWithoutAppointmentIdDTO(
    Guid Id, 
    string Medication, 
    string Dosage, 
    string Duration, 
    string Instructions);

