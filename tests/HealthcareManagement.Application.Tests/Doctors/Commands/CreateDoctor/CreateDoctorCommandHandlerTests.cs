using HealthcareManagement.Application.Doctors.Commands.CreateDoctor;
using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Exceptions.Shared;
using HealthcareManagement.Domain.Interfaces;
using NSubstitute;

namespace HealthcareManagement.Application.Tests.Doctors.Commands.CreateDoctor;

public class CreateDoctorCommandHandlerTests
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IDoctorValidationService _doctorValidationService;
    private readonly CreateDoctorCommandHandler _handler;
    private readonly string _firstName = "John";
    private readonly string _lastName = "Doe";
    private readonly DateOnly _birthdate = new DateOnly(1990, 10, 21);
    private readonly string _cpf = "12345678909";
    private readonly string _email = "john_doe@email.com";
    private readonly string _phoneNumber = "11987654321";

    public CreateDoctorCommandHandlerTests()
    {
        _doctorRepository = Substitute.For<IDoctorRepository>();
        _doctorValidationService = Substitute.For<IDoctorValidationService>();
        _handler = new CreateDoctorCommandHandler(_doctorRepository, _doctorValidationService);
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsValidationsAndReturnsId()
    {
        // Arrange
        var command = new CreateDoctorCommand(_firstName, _lastName, _birthdate, _cpf, _phoneNumber, _email);
        var expectedId = Guid.NewGuid();

        _doctorValidationService.CheckCpfUniqueAsync(command.Cpf).Returns(Task.CompletedTask);
        _doctorValidationService.CheckEmailUniqueAsync(command.Email).Returns(Task.CompletedTask);

        Doctor capturedDoctor = null; 
        _doctorRepository.CreateAsync(Arg.Do<Doctor>(d => capturedDoctor = d)).Returns(expectedId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedId, result);
        await _doctorValidationService.Received(1).CheckCpfUniqueAsync(command.Cpf);
        await _doctorValidationService.Received(1).CheckEmailUniqueAsync(command.Email);
        await _doctorRepository.Received(1).CreateAsync(Arg.Any<Doctor>());

        Assert.NotNull(capturedDoctor);
        Assert.Equal(_firstName, capturedDoctor.PersonInfo.FullName.FirstName);
        Assert.Equal(_lastName, capturedDoctor.PersonInfo.FullName.LastName);
        Assert.Equal(_birthdate, capturedDoctor.PersonInfo.BirthDate.Date);
        Assert.Equal(_cpf, capturedDoctor.PersonInfo.Cpf.Number);
        Assert.Equal(_email, capturedDoctor.PersonInfo.Email.Adress);
        Assert.Equal(_phoneNumber, capturedDoctor.PersonInfo.MobilePhoneNumber.Number);
    }

    [Fact]
    public async Task Handle_DuplicateCpf_ThrowsException()
    {
        // Arrange
        var command = new CreateDoctorCommand(_firstName, _lastName, _birthdate, _cpf, _phoneNumber, _email);

        _doctorValidationService.CheckCpfUniqueAsync(command.Cpf).Returns(Task.FromException(new CpfNotUniqueException("Cpf must be unique")));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CpfNotUniqueException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal("Cpf must be unique", exception.Message);

        await _doctorValidationService.Received(1).CheckCpfUniqueAsync(command.Cpf);
        await _doctorValidationService.DidNotReceive().CheckEmailUniqueAsync(command.Email);
        await _doctorRepository.DidNotReceive().CreateAsync(Arg.Any<Doctor>());
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ThrowsEmailNotUniqueException()
    {
        // Arrange
        var command = new CreateDoctorCommand(_firstName, _lastName, _birthdate, _cpf, _phoneNumber, _email);
        _doctorValidationService.CheckCpfUniqueAsync(command.Cpf).Returns(Task.CompletedTask);
        _doctorValidationService.CheckEmailUniqueAsync(command.Email).Returns(Task.FromException(new EmailNotUniqueException("Email must be unique")));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EmailNotUniqueException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal("Email must be unique", exception.Message);

        await _doctorValidationService.Received(1).CheckCpfUniqueAsync(command.Cpf);
        await _doctorValidationService.Received(1).CheckEmailUniqueAsync(command.Email);
        await _doctorRepository.DidNotReceive().CreateAsync(Arg.Any<Doctor>());
    }
}
