using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Domain.ValueObjects;
using MediatR;

namespace HealthcareManagement.Application.Doctors.Commands.UpdateDoctor;

public class UpdateDoctorCommandHandler : IRequestHandler<UpdateDoctorCommand, Unit>
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IDoctorValidationService _doctorValidationService;

    public UpdateDoctorCommandHandler(IDoctorRepository doctorRepository, IDoctorValidationService doctorValidationService)
    {
        _doctorRepository = doctorRepository;
        _doctorValidationService = doctorValidationService;
    }

    public async Task<Unit> Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _doctorValidationService.GetDoctorOrThrowAsync(request.Id);

        await _doctorValidationService.CheckCpfUniqueAsync(request.Cpf);
        await _doctorValidationService.CheckEmailUniqueAsync(request.Email);

        doctor.Update(
            PersonInfo.Create(
                FullName.Create(request.FirstName, request.LastName),
                BirthDate.Create(request.Birthdate),
                Cpf.Create(request.Cpf),
                Email.Create(request.Email),
                MobilePhoneNumber.Create(request.PhoneNumber)
            )
        );

        await _doctorRepository.UpdateAsync(doctor);

        return Unit.Value;
    }
}


