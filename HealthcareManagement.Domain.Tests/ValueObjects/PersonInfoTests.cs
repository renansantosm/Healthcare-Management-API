using HealthcareManagement.Domain.Validation;
using HealthcareManagement.Domain.ValueObjects;

namespace HealthcareManagement.Domain.Tests.ValueObjects;

public class PersonInfoTests
{
    private readonly FullName _fullName = FullName.Create("John", "Doe");
    private readonly FullName _fullName2 = FullName.Create("Jane", "Doe");
    private readonly BirthDate _birthDate = BirthDate.Create(new DateOnly(1990, 10, 21));
    private readonly Cpf _cpf = Cpf.Create("12345678901");
    private readonly Email _email = Email.Create("john@email.com");
    private readonly MobilePhoneNumber _mobilePhoneNumber = MobilePhoneNumber.Create("11987654321");

    [Fact]
    public void CreatePersonInfo_WithValidParameters_ShouldCreate()
    {
        var personInfo = PersonInfo.Create(_fullName, _birthDate, _cpf, _email, _mobilePhoneNumber);

        Assert.NotNull(personInfo);
        Assert.Equal(_fullName, personInfo.FullName);
        Assert.Equal(_birthDate, personInfo.BirthDate);
        Assert.Equal(_cpf, personInfo.Cpf);
        Assert.Equal(_email, personInfo.Email);
        Assert.Equal(_mobilePhoneNumber, personInfo.MobilePhoneNumber);
    }

    [Fact]
    public void CreatePersonInfo_WithNullFullName_ShouldThrowDomainValidationException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => PersonInfo.Create(null, _birthDate, _cpf, _email, _mobilePhoneNumber));

        Assert.Equal("Full name is required", exception.Message);
    }

    [Fact]
    public void CreatePersonInfo_WithNullBirthDate_ShouldThrowDomainValidationException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => PersonInfo.Create(_fullName, null, _cpf, _email, _mobilePhoneNumber));

        Assert.Equal("Birth date is required", exception.Message);
    }

    [Fact]
    public void CreatePersonInfo_WithNullCpf_ShouldThrowDomainValidationException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => PersonInfo.Create(_fullName, _birthDate, null, _email, _mobilePhoneNumber));

        Assert.Equal("CPF is required", exception.Message);
    }

    [Fact]
    public void CreatePersonInfo_WithNullEmail_ShouldThrowDomainValidationException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => PersonInfo.Create(_fullName, _birthDate, _cpf, null, _mobilePhoneNumber));

        Assert.Equal("Email is required", exception.Message);
    }

    [Fact]
    public void CreatePersonInfo_WithNullMobilePhoneNumber_ShouldThrowDomainValidationException()
    {
        var exception = Assert.Throws<DomainValidationException>(() => PersonInfo.Create(_fullName, _birthDate, _cpf, _email, null));

        Assert.Equal("Mobile phone number is required", exception.Message);
    }

    [Fact]
    public void CreatePersonInfo_WithNullParameters_ShouldThrowDomainValidationException()
    {
        var exceptions = Assert.Throws<DomainValidationException>(() => PersonInfo.Create(null, null, null, null, null));

        Assert.Equal("Full name is required", exceptions.Message);
    }

    [Fact]
    public void Equals_WithSamePersonInfo_ShouldReturnTrue()
    {
        var personInfo1 = PersonInfo.Create(_fullName, _birthDate, _cpf, _email, _mobilePhoneNumber);
        var personInfo2 = PersonInfo.Create(_fullName, _birthDate, _cpf, _email, _mobilePhoneNumber);

        Assert.True(personInfo1.Equals(personInfo2));
    }

    [Fact]
    public void Equals_WithDifferentPersonInfo_ShouldReturnFalse()
    {
        var personInfo1 = PersonInfo.Create(_fullName, _birthDate, _cpf, _email, _mobilePhoneNumber);
        var personInfo2 = PersonInfo.Create(_fullName2, _birthDate, _cpf, _email, _mobilePhoneNumber);

        Assert.False(personInfo1.Equals(personInfo2));
    }
} 
