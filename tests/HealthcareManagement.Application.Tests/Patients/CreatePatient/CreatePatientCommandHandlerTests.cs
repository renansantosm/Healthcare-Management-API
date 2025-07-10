using HealthcareManagement.Application.Doctors.Commands.CreateDoctor;
using HealthcareManagement.Application.Patients.Commands.CreatePatient;
using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Application.Services.Validation;
using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Exceptions.Shared;
using HealthcareManagement.Domain.Interfaces;
using NSubstitute;

namespace HealthcareManagement.Application.Tests.Patients.CreatePatient;

public class CreatePatientCommandHandlerTests
{
    private readonly IPatientRepository _patientRepository;
    private readonly IPatientValidationService _patientValidationService;
    private readonly CreatePatientCommandHandler _handler;
    private readonly string _firstName = "John";
    private readonly string _lastName = "Doe";
    private readonly DateOnly _birthdate = new DateOnly(1990, 10, 21);
    private readonly string _cpf = "12345678909";
    private readonly string _email = "john_doe@email.com";
    private readonly string _phoneNumber = "11987654321";

    public CreatePatientCommandHandlerTests()
    {
        _patientRepository = Substitute.For<IPatientRepository>();
        _patientValidationService = Substitute.For<IPatientValidationService>();
        _handler = new CreatePatientCommandHandler(_patientRepository, _patientValidationService);
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsValidationsAndReturnsId()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        var command = new CreatePatientCommand(_firstName, _lastName, _birthdate, _cpf, _phoneNumber, _email);

        _patientValidationService.CheckCpfUniqueAsync(command.Cpf).Returns(Task.CompletedTask);
        _patientValidationService.CheckEmailUniqueAsync(command.Email).Returns(Task.CompletedTask);

        Patient capturedPatient = null;
        _patientRepository.CreateAsync(Arg.Do<Patient>(p => capturedPatient = p)).Returns(expectedId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedId, result);
        await _patientValidationService.Received(1).CheckCpfUniqueAsync(command.Cpf);
        await _patientValidationService.Received(1).CheckEmailUniqueAsync(command.Email);
        await _patientRepository.Received(1).CreateAsync(Arg.Any<Patient>());

        Assert.NotNull(capturedPatient);
        Assert.Equal(_firstName, capturedPatient.PersonInfo.FullName.FirstName);
        Assert.Equal(_lastName, capturedPatient.PersonInfo.FullName.LastName);
        Assert.Equal(_birthdate, capturedPatient.PersonInfo.BirthDate.Date);
        Assert.Equal(_cpf, capturedPatient.PersonInfo.Cpf.Number);
        Assert.Equal(_email, capturedPatient.PersonInfo.Email.Adress);
        Assert.Equal(_phoneNumber, capturedPatient.PersonInfo.MobilePhoneNumber.Number);
    }

    [Fact]
    public async Task Handle_DuplicateCpf_ThrowsException()
    {
        // Arrange
        var command = new CreatePatientCommand(_firstName, _lastName, _birthdate, _cpf, _phoneNumber, _email);

        _patientValidationService.CheckCpfUniqueAsync(command.Cpf).Returns(Task.FromException(new CpfNotUniqueException("Cpf must be unique")));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CpfNotUniqueException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal("Cpf must be unique", exception.Message);

        await _patientValidationService.Received(1).CheckCpfUniqueAsync(command.Cpf);
        await _patientValidationService.DidNotReceive().CheckEmailUniqueAsync(command.Email);
        await _patientRepository.DidNotReceive().CreateAsync(Arg.Any<Patient>());
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ThrowsEmailNotUniqueException()
    {
        // Arrange
        var command = new CreatePatientCommand(_firstName, _lastName, _birthdate, _cpf, _phoneNumber, _email);
        _patientValidationService.CheckCpfUniqueAsync(command.Cpf).Returns(Task.CompletedTask);
        _patientValidationService.CheckEmailUniqueAsync(command.Email).Returns(Task.FromException(new EmailNotUniqueException("Email must be unique")));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EmailNotUniqueException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal("Email must be unique", exception.Message);

        await _patientValidationService.Received(1).CheckCpfUniqueAsync(command.Cpf);
        await _patientValidationService.Received(1).CheckEmailUniqueAsync(command.Email);
        await _patientRepository.DidNotReceive().CreateAsync(Arg.Any<Patient>());
    }
}
