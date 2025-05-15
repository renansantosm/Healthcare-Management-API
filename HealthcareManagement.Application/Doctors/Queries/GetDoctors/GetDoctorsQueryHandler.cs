using HealthcareManagement.Application.DTOs.Doctor.Queries;
using HealthcareManagement.Domain.Interfaces;
using MediatR;

namespace HealthcareManagement.Application.Doctors.Queries.GetDoctors;

public class GetDoctorsQueryHandler : IRequestHandler<GetDoctorsQuery, IEnumerable<DoctorDTO>>
{
    private readonly IDoctorRepository _doctorRepository;

    public GetDoctorsQueryHandler(IDoctorRepository doctorRepository)
    {
        _doctorRepository = doctorRepository;
    }

    public async Task<IEnumerable<DoctorDTO>> Handle(GetDoctorsQuery request, CancellationToken cancellationToken)
    {
        var doctors = await _doctorRepository.GetDoctorsAsync(request.PageNumber, request.PageSize);

        return doctors.Select(doctor => new DoctorDTO
        (
            doctor.Id,
            doctor.PersonInfo.FullName.FirstName,
            doctor.PersonInfo.FullName.LastName,
            doctor.PersonInfo.BirthDate.Date,
            doctor.PersonInfo.Cpf.Number,
            doctor.PersonInfo.Email.Adress,
            doctor.PersonInfo.MobilePhoneNumber.Number,
            doctor.Specialties.Select(s => s.Name).ToList()
        )).ToList();
    }
}
