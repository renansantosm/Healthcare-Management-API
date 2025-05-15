using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Exceptions.Appointment;
using HealthcareManagement.Domain.Interfaces;

namespace HealthcareManagement.Application.Services.Validation;

public class AppointmentValidationService : IAppointmentValidationService
{
    private readonly IAppointmentRepository _appointmentRepository;

    public AppointmentValidationService(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<Appointment> GetAppointmentOrThrowAsync(Guid id)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);

        if (appointment is null)
        {
            throw new AppointmentNotFoundException($"Appointment with ID {id} not found.");
        }

        return appointment;
    }

    public async Task<Appointment> GetAppointmentWithPrescriptionsOrThrowAsync(Guid id)
    {
        var appointment = await _appointmentRepository.GetAppointmentWithPrescriptionsAsync(id);

        if (appointment is null)
        {
            throw new AppointmentNotFoundException($"Appointment with ID {id} not found.");
        }

        return appointment;
    }

    public async Task ValidateAppointmentExistsOrThrowAsync(Guid id)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);

        if (appointment is null)
        {
            throw new AppointmentNotFoundException($"Appointment with ID {id} not found.");
        }
    }

    public async Task EnsureDoctorHasNoConflictingAppointmentsAsync(Guid doctorId, DateTimeOffset appointmentDate)
    {
        var hasConflict = await _appointmentRepository.HasConflictingAppointmentsForDoctorAsync(doctorId, appointmentDate);

        if (hasConflict)
        {
            throw new AppointmentConflictException($"The doctor with ID '{doctorId}' already has an appointment scheduled at {appointmentDate:yyyy-MM-dd HH:mm}.");
        }
    }

    public async Task EnsurePatientHasNoConflictingAppointmentsAsync(Guid patientId, DateTimeOffset appointmentDate)
    {
        var hasConflict = await _appointmentRepository.HasConflictingAppointmentsForPatientAsync(patientId, appointmentDate);

        if (hasConflict)
        {
            throw new AppointmentConflictException($"The patient with ID '{patientId}' already has an appointment scheduled at {appointmentDate:yyyy-MM-dd HH:mm}.");
        }
    }
}
