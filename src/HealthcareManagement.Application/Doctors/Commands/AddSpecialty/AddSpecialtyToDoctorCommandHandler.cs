using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Domain.ValueObjects;
using MediatR;

namespace HealthcareManagement.Application.Doctors.Commands.AddSpecialty;

public class AddSpecialtyToDoctorCommandHandler : IRequestHandler<AddSpecialtyToDoctorCommand, Unit>
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IDoctorValidationService _doctorValidationService;

    public AddSpecialtyToDoctorCommandHandler(IDoctorRepository doctorRepository, IDoctorValidationService doctorValidationService)
    {
        _doctorRepository = doctorRepository;
        _doctorValidationService = doctorValidationService;
    }

    public async Task<Unit> Handle(AddSpecialtyToDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _doctorValidationService.GetDoctorOrThrowAsync(request.DoctorId);

        doctor.AddSpecialty(Specialty.Create(request.Specialty));

        await _doctorRepository.UpdateAsync(doctor);

        return Unit.Value;
    }
}
