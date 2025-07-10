using HealthcareManagement.Domain.Entities;

namespace HealthcareManagement.Application.Services.Interfaces;

public interface IAppointmentValidationService
{
    Task<Appointment> GetAppointmentOrThrowAsync(Guid id);
    Task<Appointment> GetAppointmentWithPrescriptionsOrThrowAsync(Guid id);
    Task ValidateAppointmentExistsOrThrowAsync(Guid id);
    Task EnsureDoctorHasNoConflictingAppointmentsAsync(Guid doctorId, DateTimeOffset appointmentDate);
    Task EnsurePatientHasNoConflictingAppointmentsAsync(Guid patientId, DateTimeOffset appointmentDate);
}
