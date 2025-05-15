using HealthcareManagement.Application.Services.Validation;
using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Exceptions.Doctor;
using HealthcareManagement.Domain.Exceptions.Shared;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Domain.ValueObjects;
using NSubstitute;

namespace HealthcareManagement.Application.Tests.Services;

public class DoctorValidationServiceTests
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly DoctorValidationService _service;

    private static readonly FullName _fullName = FullName.Create("John", "Doe");
    private static readonly BirthDate _birthDate = BirthDate.Create(new DateOnly(1990, 10, 21));
    private static readonly Cpf _cpf = Cpf.Create("12345678901");
    private static readonly Email _email = Email.Create("john@email.com");
    private static readonly MobilePhoneNumber _mobilePhoneNumber = MobilePhoneNumber.Create("11987654321");
    private static readonly PersonInfo _personInfo = PersonInfo.Create(_fullName, _birthDate, _cpf, _email, _mobilePhoneNumber);

    public DoctorValidationServiceTests()
    {
        _doctorRepository = Substitute.For<IDoctorRepository>();
        _service = new DoctorValidationService(_doctorRepository);
    }

    [Fact]
    public async Task GetDoctorOrThrowAsync_WhenDoctorExists_ReturnsDoctor()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var expectedDoctor = new Doctor(doctorId, _personInfo);
        _doctorRepository.GetByIdAsync(doctorId).Returns(Task.FromResult(expectedDoctor));

        // Act
        var result = await _service.GetDoctorOrThrowAsync(doctorId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDoctor, result);
    }

    [Fact]
    public async Task GetDoctorOrThrowAsync_WhenDoctorDoesNotExist_ThrowsDoctorNotFoundException()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        _doctorRepository.GetByIdAsync(doctorId).Returns(Task.FromResult<Doctor>(null));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DoctorNotFoundException>(() => _service.GetDoctorOrThrowAsync(doctorId));
        Assert.Equal($"Doctor with ID {doctorId} not found.", exception.Message);
    }

    [Fact]
    public async Task CheckCpfUniqueAsync_WhenCpfIsUnique_DoesNotThrow()
    {
        // Arrange
        _doctorRepository.CpfExistsAsync(_cpf.Number).Returns(false);

        // Act & Assert
        await _service.CheckCpfUniqueAsync(_cpf.Number);
    }

    [Fact]
    public async Task CheckCpfUniqueAsync_WhenCpfExists_ThrowsCpfNotUniqueException()
    {
        // Arrange
        _doctorRepository.CpfExistsAsync(_cpf.Number).Returns(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CpfNotUniqueException>(() => _service.CheckCpfUniqueAsync(_cpf.Number));
        Assert.Equal("CPF must be unique", exception.Message);
    }

    [Fact]
    public async Task CheckEmailUniqueAsync_WhenEmailIsUnique_DoesNotThrow()
    {
        // Arrange
        _doctorRepository.EmailExistsAsync(_email.Adress).Returns(false);

        // Act & Assert
        await _service.CheckEmailUniqueAsync(_email.Adress);
    }

    [Fact]
    public async Task CheckEmailUniqueAsync_WhenEmailExists_ThrowsEmailNotUniqueException()
    {
        // Arrange
        _doctorRepository.EmailExistsAsync(_email.Adress).Returns(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EmailNotUniqueException>(() => _service.CheckEmailUniqueAsync(_email.Adress));
        Assert.Equal("Email must be unique", exception.Message);
    }
}
