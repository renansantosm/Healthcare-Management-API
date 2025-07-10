using HealthcareManagement.Application.DTOs.Prescription;
using HealthcareManagement.Domain.Enums;

namespace HealthcareManagement.Application.DTOs.Appointment.Queries;

public record AppointmentDTO(
    Guid Id,
    Guid DoctorId,
    Guid PatientId, 
    string AppointmentDate, 
    EAppointmentStatus Status);
