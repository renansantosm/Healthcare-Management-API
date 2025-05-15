using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Domain.ValueObjects;
using MediatR;

namespace HealthcareManagement.Application.Doctors.Commands.CreateDoctor;

public class CreateDoctorCommandHandler : IRequestHandler<CreateDoctorCommand, Guid>
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IDoctorValidationService _doctorValidationService;

    public CreateDoctorCommandHandler(IDoctorRepository doctorRepository, IDoctorValidationService doctorValidationService)
    {
        _doctorRepository = doctorRepository;
        _doctorValidationService = doctorValidationService;
    }

    public async Task<Guid> Handle(CreateDoctorCommand request, CancellationToken cancellationToken)
    {
        await _doctorValidationService.CheckCpfUniqueAsync(request.Cpf);
        await _doctorValidationService.CheckEmailUniqueAsync(request.Email);

        var doctor = new Doctor(
            Guid.NewGuid(),
            PersonInfo.Create(
                FullName.Create(request.FirstName, request.LastName),
                BirthDate.Create(request.Birthdate),
                Cpf.Create(request.Cpf),
                Email.Create(request.Email),
                MobilePhoneNumber.Create(request.PhoneNumber)
            )
        );

        var id = await _doctorRepository.CreateAsync(doctor);

        return id;
    }
}
