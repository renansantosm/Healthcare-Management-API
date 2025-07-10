using HealthcareManagement.Application.DTOs.Appointment.Queries;
using HealthcareManagement.Application.Services.Interfaces;
using MediatR;

namespace HealthcareManagement.Application.Appointments.Queries.GetAppointmentById;

public class GetAppointmentByIdQueryHandler : IRequestHandler<GetAppointmentByIdQuery, AppointmentDTO?>
{
    private readonly IAppointmentValidationService _appointmentValidationService;

    public GetAppointmentByIdQueryHandler(IAppointmentValidationService appointmentValidationService)
    {
        _appointmentValidationService = appointmentValidationService;
    }

    public async Task<AppointmentDTO> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentValidationService.GetAppointmentOrThrowAsync(request.Id);

        return new AppointmentDTO
        (
            appointment.Id,
            appointment.DoctorId,
            appointment.PatientId,
            appointment.AppointmentDate.Date.ToString("dd/MM/yyyy HH:mm:ss"),
            appointment.Status
        );
    }
}
