using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Enums;
using HealthcareManagement.Domain.Exceptions.Appointment;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace HealthcareManagement.Infra.Data.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly AppDbContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;
    private const int AppointmentDurationMinutes = 30;

    public AppointmentRepository(AppDbContext context, IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<IEnumerable<Appointment>> GetAppointmentsAsync(int pageNumber, int pageSize)
    {
        return await _context.Appointments
            .AsNoTracking().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<Appointment?> GetByIdAsync(Guid id)
    {
        return await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Appointment?> GetAppointmentWithPrescriptionsAsync(Guid id)
    {
        return await _context.Appointments
            .Include(a => a.Prescriptions)
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Appointment?> GetAppointmentWithDetailsAsync(Guid id)
    {
        return await _context.Appointments
            .AsNoTracking()
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .Include(a => a.Prescriptions)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<bool> HasConflictingAppointmentsForDoctorAsync(Guid doctorId, DateTimeOffset newAppointmentDate)
    {
        // Consultas marcadas tem duração de 30 minutos
        var newAppointmentEnd = newAppointmentDate.AddMinutes(AppointmentDurationMinutes);

        return await _context.Appointments
            .AsNoTracking()
            .Where(a => a.DoctorId == doctorId)
            .Where(a => a.Status == EAppointmentStatus.Scheduled)
            .AnyAsync(a =>
                newAppointmentDate < a.AppointmentDate.Date.AddMinutes(AppointmentDurationMinutes) &&
                a.AppointmentDate.Date < newAppointmentEnd
            );
    }

    public async Task<bool> HasConflictingAppointmentsForPatientAsync(Guid patientId, DateTimeOffset newAppointmentDate)
    {
        // Consultas marcadas tem duração de 30 minutos
        var newAppointmentEnd = newAppointmentDate.AddMinutes(AppointmentDurationMinutes);

        return await _context.Appointments
            .AsNoTracking()
            .Where(a => a.PatientId == patientId)
            .Where(a => a.Status == EAppointmentStatus.Scheduled)
            .AnyAsync(a =>
                newAppointmentDate < a.AppointmentDate.Date.AddMinutes(AppointmentDurationMinutes) &&
                a.AppointmentDate.Date < newAppointmentEnd
            ); 
    }

    public async Task<bool> CanCancelAppointment(Guid id)
    {
        var currentTime = _dateTimeProvider.GetUtcNow();

        return await _context.Appointments.AsNoTracking()
            .AnyAsync(a => a.Id == id && 
                a.AppointmentDate.Date > currentTime.AddHours(24) && 
                a.Status == EAppointmentStatus.Scheduled);
    }

    public async Task<Guid> CreateAsync(Appointment appointment)
    {
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();
        return appointment.Id;
    }

    public async Task UpdateAsync(Appointment appointment)
    {
        _context.Appointments.Update(appointment);
        await _context.SaveChangesAsync();
    }

    public async Task AddPrescriptionAsync(Guid appointmentId, Prescription prescription)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Prescriptions)
            .FirstOrDefaultAsync(a => a.Id == appointmentId);

        appointment.AddPrescription(prescription);

        _context.Set<Prescription>().Add(prescription);

        await _context.SaveChangesAsync();
    }
}
