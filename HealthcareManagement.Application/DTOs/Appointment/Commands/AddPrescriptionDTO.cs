namespace HealthcareManagement.Application.DTOs.Appointment.Commands;

public record AddPrescriptionDTO(
    Guid AppointmentId, 
    string Medication, 
    string Dosage, 
    string Duration, 
    string Instructions);
