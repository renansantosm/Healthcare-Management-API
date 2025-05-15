using HealthcareManagement.Application.DTOs.Doctor.Queries;
using MediatR;

namespace HealthcareManagement.Application.Doctors.Queries.GetDoctorById;

public record GetDoctorByIdQuery(Guid Id) : IRequest<DoctorDTO?>;
