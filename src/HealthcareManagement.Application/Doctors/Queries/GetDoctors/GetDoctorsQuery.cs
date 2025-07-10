using HealthcareManagement.Application.DTOs.Doctor.Queries;
using MediatR;

namespace HealthcareManagement.Application.Doctors.Queries.GetDoctors;

public record GetDoctorsQuery(
    int PageNumber, 
    int PageSize) : IRequest<IEnumerable<DoctorDTO>>;
