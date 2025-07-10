using HealthcareManagement.Domain.Entities;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Domain.Validation;
using HealthcareManagement.Domain.ValueObjects;
using NSubstitute;

namespace HealthcareManagement.Domain.Tests.Entities;

public class PatientTest
{
    private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();

    private static readonly FullName _fullName = FullName.Create("John", "Doe");
    private static readonly BirthDate _birthDate = BirthDate.Create(new DateOnly(1990, 10, 21));
    private static readonly Cpf _cpf = Cpf.Create("12345678901");
    private static readonly Email _email = Email.Create("john@email.com");
    private static readonly MobilePhoneNumber _mobilePhoneNumber = MobilePhoneNumber.Create("11987654321");
    private static readonly PersonInfo _personInfo = PersonInfo.Create(_fullName, _birthDate, _cpf, _email, _mobilePhoneNumber);
    private static readonly Guid _guid = Guid.Parse("d2719f5a-3c40-4c2e-9c75-9d9a3b6bfb1d");
    private static readonly Guid _emptyGuid = Guid.Empty;

    [Fact]
    public void CreatePatient_ValidData_ShouldCreatePatient()
    {
        // Arrange
        var patient = new Patient(_guid, _personInfo);

        // Act
        var result = patient.PersonInfo;

        // Assert
        Assert.Equal(_personInfo, result);
        Assert.Equal(_guid, patient.Id);
        Assert.Empty(patient.Appointments);
    }

    [Fact]
    public void CreatePatient_WithEmptyId_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => new Patient(_emptyGuid, _personInfo));

        Assert.Equal("Id cannot be empty", exception.Message);
    }

    [Fact]
    public void UpdatePatient_ValidData_ShouldUpdatePatient()
    {
        // Arrange
        var patient = new Patient(_guid, _personInfo);
        var newFullName = FullName.Create("Jane", "Doe");
        var newPersonInfo = PersonInfo.Create(newFullName, _birthDate, _cpf, _email, _mobilePhoneNumber);

        // Act
        patient.Update(newPersonInfo);

        // Assert
        Assert.Equal(newPersonInfo, patient.PersonInfo);
        Assert.Equal(_guid, patient.Id);
        Assert.Empty(patient.Appointments);
    }
}
