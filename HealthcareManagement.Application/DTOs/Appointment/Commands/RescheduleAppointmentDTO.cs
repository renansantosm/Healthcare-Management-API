namespace HealthcareManagement.Application.DTOs.Appointment.Commands;

public record RescheduleAppointmentDTO(
    Guid Id, 
    DateTimeOffset NewAppointmentDate);

