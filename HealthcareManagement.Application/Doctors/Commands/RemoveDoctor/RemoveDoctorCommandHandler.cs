using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Domain.Interfaces;
using MediatR;

namespace HealthcareManagement.Application.Doctors.Commands.RemoveDoctor;

public class RemoveDoctorCommandHandler : IRequestHandler<RemoveDoctorCommand, Unit>
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IDoctorValidationService _doctorValidationService;

    public RemoveDoctorCommandHandler(IDoctorRepository doctorRepository, IDoctorValidationService doctorValidationService)
    {
        _doctorRepository = doctorRepository;
        _doctorValidationService = doctorValidationService;
    }

    public async Task<Unit> Handle(RemoveDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _doctorValidationService.GetDoctorOrThrowAsync(request.Id);

        await _doctorRepository.DeleteAsync(doctor);

        return Unit.Value;
    }
}
