using HealthcareManagement.Domain.Entities;

namespace HealthcareManagement.Domain.Interfaces;

public interface IAppointmentRepository
{
    Task<IEnumerable<Appointment>> GetAppointmentsAsync(int pageNumber, int pageSize);
    Task<Appointment?> GetByIdAsync(Guid id);
    Task<Appointment?> GetAppointmentWithPrescriptionsAsync(Guid id);
    Task<Appointment?> GetAppointmentWithDetailsAsync(Guid id);
    Task<bool> HasConflictingAppointmentsForDoctorAsync(Guid doctorId, DateTimeOffset appointmentDate);
    Task<bool> HasConflictingAppointmentsForPatientAsync(Guid patientId, DateTimeOffset appointmentDate);
    Task<bool> CanCancelAppointment(Guid id);  
    Task<Guid> CreateAsync(Appointment appointment);
    Task UpdateAsync(Appointment appointment);
    Task AddPrescriptionAsync(Guid appointmentId, Prescription prescription);
}
