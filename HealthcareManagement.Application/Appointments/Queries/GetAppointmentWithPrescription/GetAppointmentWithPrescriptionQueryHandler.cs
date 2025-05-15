using HealthcareManagement.Application.DTOs.Appointment.Queries;
using HealthcareManagement.Application.DTOs.Prescription.Queries;
using HealthcareManagement.Application.Services.Interfaces;
using MediatR;

namespace HealthcareManagement.Application.Appointments.Queries.GetAppointmentWithPrescription;

public class GetAppointmentWithPrescriptionQueryHandler : IRequestHandler<GetAppointmentWithPrescriptionQuery, AppointmentWithPrescriptionDTO?>
{
    private readonly IAppointmentValidationService _appointmentValidationService;

    public GetAppointmentWithPrescriptionQueryHandler(IAppointmentValidationService appointmentValidationService)
    {
        _appointmentValidationService = appointmentValidationService;
    }

    public async Task<AppointmentWithPrescriptionDTO?> Handle(GetAppointmentWithPrescriptionQuery request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentValidationService.GetAppointmentWithPrescriptionsOrThrowAsync(request.Id);

        return new AppointmentWithPrescriptionDTO(
            appointment.Id,
            appointment.DoctorId,
            $"{appointment.Doctor.PersonInfo.FullName.FirstName} {appointment.Doctor.PersonInfo.FullName.LastName}",
            appointment.PatientId,
            $"{appointment.Patient.PersonInfo.FullName.FirstName} {appointment.Patient.PersonInfo.FullName.LastName}",
            appointment.AppointmentDate.Date.ToString("dd/MM/yyyy HH:mm:ss"),
            appointment.Prescriptions.Select(p => new PrescriptionWithoutAppointmentIdDTO(
                p.Id,
                p.Medication,
                p.Dosage,
                p.Duration,
                p.Instructions
            )).ToList());
    }
}
