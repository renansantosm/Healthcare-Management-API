namespace HealthcareManagement.Application.DTOs.Appointment.Commands;

public record UpdatePrescriptionDTO(
    Guid AppointmentId,
    Guid PrescriptionId,
    string Medication,
    string Dosage,
    string Duration,
    string Instructions);
