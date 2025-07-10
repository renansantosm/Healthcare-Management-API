using HealthcareManagement.Application.DTOs.Doctor.Queries;
using HealthcareManagement.Application.Services.Interfaces;
using MediatR;

namespace HealthcareManagement.Application.Doctors.Queries.GetDoctorById;

public class GetDoctorByIdQueryHandler : IRequestHandler<GetDoctorByIdQuery, DoctorDTO?>
{
    private readonly IDoctorValidationService _doctorValidationService;

    public GetDoctorByIdQueryHandler(IDoctorValidationService doctorValidationService)
    {
        _doctorValidationService = doctorValidationService;
    }

    public async Task<DoctorDTO?> Handle(GetDoctorByIdQuery request, CancellationToken cancellationToken)
    {
        var doctor = await _doctorValidationService.GetDoctorOrThrowAsync(request.Id);

        return new DoctorDTO
        (
            doctor.Id,
            doctor.PersonInfo.FullName.FirstName,
            doctor.PersonInfo.FullName.LastName,
            doctor.PersonInfo.BirthDate.Date,
            doctor.PersonInfo.Cpf.Number,
            doctor.PersonInfo.Email.Adress,
            doctor.PersonInfo.MobilePhoneNumber.Number,
            doctor.Specialties.Select(s => s.Name).ToList()
        ); 
    }
}
