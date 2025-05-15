using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Domain.Validation;
using HealthcareManagement.Domain.ValueObjects;
using NSubstitute;

namespace HealthcareManagement.Domain.Tests.Entities;

public class DoctorTests
{
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();

    private static readonly Guid _guid = Guid.Parse("d2719f5a-3c40-4c2e-9c75-9d9a3b6bfb1d");
    private static readonly Guid _emptyGuid = Guid.Empty;   
    private static readonly FullName _fullName = FullName.Create("John", "Doe");
    private static readonly BirthDate _birthDate = BirthDate.Create(new DateOnly(1990, 10, 21));
    private static readonly Cpf _cpf = Cpf.Create("12345678901");
    private static readonly Email _email = Email.Create("john@email.com");
    private static readonly MobilePhoneNumber _mobilePhoneNumber = MobilePhoneNumber.Create("11987654321");
    private static readonly PersonInfo _personInfo = PersonInfo.Create(_fullName, _birthDate, _cpf, _email, _mobilePhoneNumber);
    private static readonly Specialty specialty = Specialty.Create("Cardiology");
    private static readonly Specialty specialty2 = Specialty.Create("Dermatology");
    private static readonly Specialty specialty3 = Specialty.Create("Pediatrics");

    [Fact]
    public void CreateDoctor_ValidData_ShouldCreateDoctor()
    {
        // Arrange
        var doctor = new Doctor(_guid, _personInfo);

        // Act
        var result = doctor.PersonInfo;

        // Assert
        Assert.Equal(_personInfo, result);
        Assert.Equal(_guid, doctor.Id);
        Assert.Empty(doctor.Specialties);
        Assert.Empty(doctor.Appointments);
    }

    [Fact]
    public void CreateDoctor_WithEmptyId_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => new Doctor(_emptyGuid, _personInfo));

        Assert.Equal("Id cannot be empty", exception.Message);
    }

    [Fact]
    public void UpdateDoctor_ValidData_ShouldUpdateDoctor()
    {
        // Arrange
        var doctor = new Doctor(_guid, _personInfo);
        var newFullName = FullName.Create("Jane", "Doe");
        var newPersonInfo = PersonInfo.Create(newFullName, _birthDate, _cpf, _email, _mobilePhoneNumber);

        // Act
        doctor.Update(newPersonInfo);

        // Assert
        Assert.Equal(newPersonInfo, doctor.PersonInfo);
        Assert.Equal(_guid, doctor.Id);
        Assert.Empty(doctor.Specialties);
        Assert.Empty(doctor.Appointments);
    }

    [Fact]
    public void AddSpecialty_ValidData_ShouldAddSpecialty()
    {
        // Arrange
        var doctor = new Doctor(_guid, _personInfo);

        // Act
        doctor.AddSpecialty(specialty);
        doctor.AddSpecialty(specialty2);

        // Assert
        Assert.Contains(specialty, doctor.Specialties);
        Assert.Contains(specialty2, doctor.Specialties);
    }

    [Fact]
    public void AddSpecialty_ExceedingMaxSpecialties_ShouldThrowDomainException()
    {
        // Arrange
        var doctor = new Doctor(_guid, _personInfo);
        doctor.AddSpecialty(specialty);
        doctor.AddSpecialty(specialty2);

        // Act & Assert
        var exception = Assert.Throws<DomainValidationException>(() => doctor.AddSpecialty(specialty3));
        Assert.Equal("A doctor cannot have more than two specialties", exception.Message);
    }

    [Fact]
    public void AddSpecialty_AlreadyHasSpecialty_ShouldThrowDomainException()
    {
        // Arrange
        var doctor = new Doctor(_guid, _personInfo);
        doctor.AddSpecialty(specialty);

        // Act & Assert
        var exception = Assert.Throws<DomainValidationException>(() => doctor.AddSpecialty(specialty));
        Assert.Equal("This specialty is already registered for this doctor.", exception.Message);
    }

    [Fact]
    public void RemoveSpecialty_ValidData_ShouldRemoveSpecialty()
    {
        // Arrange
        var doctor = new Doctor(_guid, _personInfo);
        doctor.AddSpecialty(specialty);
        doctor.AddSpecialty(specialty2);

        // Act
        doctor.RemoveSpecialty(specialty);

        // Assert
        Assert.DoesNotContain(specialty, doctor.Specialties);
        Assert.Contains(specialty2, doctor.Specialties);
    }

    [Fact]
    public void RemoveSpecialty_NonExistentSpecialty_ShouldThrowDomainException()
    {
        // Arrange
        var doctor = new Doctor(_guid, _personInfo);
        doctor.AddSpecialty(specialty);

        // Act & Assert
        var exception = Assert.Throws<DomainValidationException>(() => doctor.RemoveSpecialty(specialty2));
        Assert.Equal("Specialty not found for this doctor.", exception.Message);
    }
}
