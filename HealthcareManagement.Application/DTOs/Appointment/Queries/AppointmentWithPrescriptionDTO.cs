using HealthcareManagement.Application.DTOs.Prescription.Queries;

namespace HealthcareManagement.Application.DTOs.Appointment.Queries;

public record AppointmentWithPrescriptionDTO(
    Guid Id,
    Guid DoctorId, 
    string DoctorName,
    Guid PatientId, 
    string PatientName, 
    string AppointmentDate, 
    IEnumerable<PrescriptionWithoutAppointmentIdDTO> Prescriptions
);

