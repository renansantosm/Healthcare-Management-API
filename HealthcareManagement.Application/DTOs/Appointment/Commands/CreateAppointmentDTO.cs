namespace HealthcareManagement.Application.DTOs.Appointment.Commands;

public record CreateAppointmentDTO(
    Guid DoctorId,
    Guid PatientId, 
    DateTimeOffset AppointmentDate);

