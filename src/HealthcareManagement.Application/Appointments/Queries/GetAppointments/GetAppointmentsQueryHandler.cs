using HealthcareManagement.Application.DTOs.Appointment.Queries;
using HealthcareManagement.Domain.Interfaces;
using MediatR;

namespace HealthcareManagement.Application.Appointments.Queries.GetAppointments;

public class GetAppointmentsQueryHandler : IRequestHandler<GetAppointmentsQuery, IEnumerable<AppointmentDTO>>
{
    private readonly IAppointmentRepository _repository;

    public GetAppointmentsQueryHandler(IAppointmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AppointmentDTO>> Handle(GetAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var appointments = await _repository.GetAppointmentsAsync(request.PageNumber, request.PageSize);

        return appointments.Select(a => new AppointmentDTO
        (
            a.Id,
            a.DoctorId,
            a.PatientId,
            a.AppointmentDate.Date.ToString("dd/MM/yyyy HH:mm:ss"),
            a.Status
        )).ToList();
    }
}